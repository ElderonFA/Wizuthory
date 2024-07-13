using System;
using Cinemachine;
using Looting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static Action exitToMenu;
    public static Action restartLvl;
    public static Action<int> toNewLevel;
    
    private GameObject currentCamera;
    private GameObject confObj;

    public static PlayerController playerControllerInstance;
    private HealthBarController interfaceControllerInstance;
    private CameraHandler cameraHandlerInstance;
    private CutSceneController cutSceneController;
    
    private UiSkillsController uiSkillsControllerInstance;
    
    public static Action<PlayerController> actionGetPlayerController;
    public static Action<HealthBarController> actionGetInterfaceController;
    public static Action<CameraHandler> actionGetCameraHandler;
    public static Action<CutSceneController> actionGetCutSceneController;
    
    public static Action<UiSkillsController> actionGetUiSkillsController;
    public static Action<PlayerSkills> actionUnlockUiSkill;
    public static Action<PlayerSkills> actionUseUiSkill;
    
    public static Action<Health, string> actionShowBossHealthBar;
    public static Action actionStartShowBloodOnBossName;

    public static Action onGameEnd;
    
    //костыль с запоминанием кол-ва зелий
    private int countHealPotionInStartLvl;
    
    //костыль с уровнями
    public static int currentLlv = 0;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        exitToMenu += LoadMenu;
        toNewLevel += LoadLevel;

        actionGetPlayerController += GetPlayerController;
        actionGetInterfaceController += GetInterfaceController;
        actionGetCameraHandler += GetCameraHandler;
        actionGetCutSceneController += GetCutSceneController;
        actionGetUiSkillsController += GetUiSkillsController;

        actionShowBossHealthBar += ShowHealthBossUI;
        actionStartShowBloodOnBossName += HideHealthBossUI;
    }

    private void GetCutSceneController(CutSceneController csc)
    {
        if (cutSceneController != null)
        {
            Destroy(csc.gameObject);
            return;
        }

        cutSceneController = csc;
    }

    private void GetUiSkillsController(UiSkillsController uiSC)
    {
        if (uiSkillsControllerInstance != null)
        {
            Destroy(uiSC.gameObject);
            return;
        }

        uiSkillsControllerInstance = uiSC;
        
        actionUnlockUiSkill += uiSkillsControllerInstance.UnlockUiSkill;
        actionUseUiSkill += uiSkillsControllerInstance.UseUiSkill;
    }

    private void GetPlayerController(PlayerController pc)
    {
        if (playerControllerInstance != null)
        {
            Destroy(pc.gameObject);
            return;
        }
        
        playerControllerInstance = pc;
    }
    
    private void GetInterfaceController(HealthBarController ic)
    {
        if (interfaceControllerInstance != null)
        {
            Destroy(ic.gameObject);
            return;
        }
        
        interfaceControllerInstance = ic;
        onGameEnd += interfaceControllerInstance.ShowEndGamePopup;
    }
    
    private void GetCameraHandler(CameraHandler ch)
    {
        if (cameraHandlerInstance != null)
        {
            Destroy(ch.gameObject);
            return;
        }
        
        cameraHandlerInstance = ch;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitToMenu?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!cutSceneController.cutSceneIsEnd)
            {
                return;
            }
            
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadLevel(2);
        }
    }
    
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerControllerInstance.SetHealPotionCount(countHealPotionInStartLvl);
        
        if (currentLlv > 1)
        {
            PlayerController.onHealPotionCountChange?.Invoke(countHealPotionInStartLvl);
        }
        
        restartLvl?.Invoke();
    }

    public void Play()
    {
        LoadLevel(1);
    }

    private void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(levelNum);
        HealthBarController.startLevelEvent?.Invoke();

        currentLlv++;

        if (currentCamera == null)
        {
            currentCamera = GameObject.FindWithTag("MainCamera");
        }

        if (currentLlv > 1)
        {
            Destroy(confObj);
        }
        
        confObj = GameObject.FindWithTag("CameraConfiner");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        DestroySelf();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowHealthBossUI(Health bossHealth, string bossName)
    {
        interfaceControllerInstance.SetBossHealth(bossHealth, "Unknow wizard");
    }
    
    private void HideHealthBossUI()
    {
        interfaceControllerInstance.ShowBloodOnBossName();
    }

    public void OnDestroy()
    {
        exitToMenu -= LoadMenu;
        toNewLevel -= LoadLevel;

        actionGetPlayerController -= GetPlayerController;
        actionGetInterfaceController -= GetInterfaceController;
        actionGetCameraHandler -= GetCameraHandler;
        actionGetCutSceneController -= GetCutSceneController;
        actionGetUiSkillsController -= GetUiSkillsController;

        actionShowBossHealthBar -= ShowHealthBossUI;
        actionStartShowBloodOnBossName -= HideHealthBossUI;
        
        actionUnlockUiSkill -= uiSkillsControllerInstance.UnlockUiSkill;
        actionUseUiSkill -= uiSkillsControllerInstance.UseUiSkill;
    }
}
