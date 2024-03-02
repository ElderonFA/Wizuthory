using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static Action exitToMenu;
    public static Action<int> toNewLevel;

    private GameObject camera;
    private GameObject confObj;
    
    //костыль с уровнями
    private int currentLlv = 0;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        exitToMenu += DestroySelf;
        toNewLevel += LoadLevel;
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
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadLevel(2);
        }
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

        if (camera == null)
        {
            camera = GameObject.FindWithTag("MainCamera");
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

    public void OnDestroy()
    {
        exitToMenu -= DestroySelf;
        toNewLevel -= LoadLevel;
    }
}
