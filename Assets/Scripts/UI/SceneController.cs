using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static Action exitToMenu;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        exitToMenu += DestroySelf;
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
    }
}
