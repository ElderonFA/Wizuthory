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

    private HealthBarController interfaceControllerInstance;
    private CameraHandler cameraHandlerInstace;
    public static PlayerController playerControllerInstance;
    private UiSkillsController uiSkillsControllerInstance;
    
    public static Action<PlayerController> actionGetPlayerController;
    public static Action<HealthBarController> actionGetInterfaceController;
    public static Action<CameraHandler> actionGetCameraHandler;
    
    public static Action<UiSkillsController> actionGetUiSkillsController;
    public static Action<PlayerSkills> actionUnlockUiSkill;
    public static Action<PlayerSkills> actionUseUiSkill;
    
    public static Action<Health, string> actionShowBossHealthBar;
    public static Action actionStartShowBloodOnBossName;
    
    //костыль с запоминанием кол-ва зелий
    private int countHealPotionInStartLvl;
    
    //костыль с уровнями
    public static int currentLlv = 0;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        exitToMenu += DestroySelf;
        toNewLevel += LoadLevel;

        actionGetPlayerController += GetPlayerController;
        actionGetInterfaceController += GetInterfaceController;
        actionGetCameraHandler += GetCameraHandler;
        actionGetUiSkillsController += GetUiSkillsController;

        actionShowBossHealthBar += ShowHealthBossUI;
        actionStartShowBloodOnBossName += HideHealthBossUI;
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
    }
    
    private void GetCameraHandler(CameraHandler ch)
    {
        if (cameraHandlerInstace != null)
        {
            Destroy(ch.gameObject);
            return;
        }
        
        cameraHandlerInstace = ch;
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
            LoadMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
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
        PlayerController.onHealPotionCountChange?.Invoke(countHealPotionInStartLvl);
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

        if (currentLlv > 1)
        {
            countHealPotionInStartLvl = playerControllerInstance.GetCountHealPotion;
            PlayerController.onHealPotionCountChange?.Invoke(countHealPotionInStartLvl);
        }

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
        exitToMenu -= DestroySelf;
        toNewLevel -= LoadLevel;
    }
}
