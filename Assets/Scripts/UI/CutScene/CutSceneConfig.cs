using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "CutScene", menuName = "Create a CutScene", order = 1)]
public class CutSceneConfig : ScriptableObject
{
    [SerializeField] private CutScene currentCutScene;

    public CutScene GetConfigCutScene => currentCutScene;
}

[Serializable]
public class CutScene
{
    [SerializeField] private CutSceneStep[] allStep;
    public CutSceneStep[] GetAllStep => allStep;
}

[Serializable]
public class CutSceneStep
{
    [SerializeField] private string text;
    public string GetText => text;
    
    [SerializeField] private Persons person;
    public Persons GetPerson => person;
}

public enum Persons
{
    VoiceOver,
    Player,
    Skeleton,
}
