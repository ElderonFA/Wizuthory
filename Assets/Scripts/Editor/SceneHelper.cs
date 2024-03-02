using System.Windows.Forms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

public class SceneHelper : EditorWindow
{
    private const string scenesPath = "Assets/Scenes/";
    private const string sceneType  = ".unity";

    bool groupEnabled;
    
    //string myString = "Hello World";
    //bool myBool = true;
    //float myFloat = 1.23f;
    
    // Add menu item named "My Window" to the Window menu
    [UnityEditor.MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SceneHelper));
    }
    
    void OnGUI()
    {
        ButtonChangeScene("Load Menu", "Menu");
        ButtonChangeScene("Load Level 1", "Level_1");
        ButtonChangeScene("Load Level 2", "Level_2");
        
        /*GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField ("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();*/
    }

    private void ButtonChangeScene(string buttonName, string sceneName)
    {
        if (GUILayout.Button(buttonName))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(scenesPath + sceneName + sceneType);
        }
    }
}