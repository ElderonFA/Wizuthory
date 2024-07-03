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
    
    [SerializeField] private CutSceneEvents sceneEvent;
    public CutSceneEvents GetEvent => sceneEvent;
}

public enum Persons
{
    VoiceOver,
    Player,
    Skeleton,
    Boss,
}

public enum CutSceneEvents
{
    None = 0,
    EndLevel = 1,
    StartLevel = 2,
    BranchCrack = 3,
    LookAtWhole = 4,
    StartFiringSpot = 5,
    WakeUpBoss = 6,
    StartUpdateBoss = 7,
    WaitForBlackScreen = 8,
    EndGame = 9,
}
