using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static Action exitToMenu;
    public static Action<int> toNewLevel;

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
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(levelNum);
        HealthBarController.startLevelEvent?.Invoke();
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
