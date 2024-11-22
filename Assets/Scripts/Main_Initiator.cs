using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Main_Initiator : MonoBehaviour
{
    [System.Serializable]
    public struct FishSetting{
        public PerRendererFish fishRenderer;
        [ColorUsage(false, true)] public Color fishColor;
        public void ApplySettings(){
            fishRenderer.EmissionColor = fishColor;
        }
    }
    [System.Serializable]
    public class SharingSets{
        public GameObject GameSets;
        public int[] miniGameIndexArray;
        public bool MatchIndex(int gameIndex){
            for(int i=0; i<miniGameIndexArray.Length; i++){
                if(miniGameIndexArray[i] == gameIndex) return true;
            }
            return false;
        }
    }
    [SerializeField] private int StartGameIndex = 0;
    [SerializeField] private bool includeLast;
    [SerializeField] private MiniGameManager miniGameManager;
[Header("Sharing Sets")]
    [SerializeField] private SharingSets[] sharingSets;
[Header("Special Sets")]
    [SerializeField] private PerRendererWater perRendererWater;
    [SerializeField] private FishSetting fishSetting;
    [SerializeField] private MiniGameBasic[] FishMiniGames;
    [SerializeField] private MiniGameBasic[] SkyMiniGames;
    [SerializeField] private MiniGameBasic controlFishGame;
    void Start()
    {
        for(int i=0; i<sharingSets.Length; i++){
            if(sharingSets[i].MatchIndex(StartGameIndex))
                sharingSets[i].GameSets.SetActive(true);
            else
                sharingSets[i].GameSets.SetActive(false);
        }

        miniGameManager.StartGame(StartGameIndex);
        if(includeLast) miniGameManager.StartGame(StartGameIndex-1);
        PerRendererWater_Initiation();
    }
    void PerRendererWater_Initiation(){
        if(controlFishGame == miniGameManager.GetGame(StartGameIndex)){
            perRendererWater.fallOffPower = 1;
            fishSetting.ApplySettings();
        }
        else{
            perRendererWater.fallOffPower = 0.05f;
        }
        foreach(var miniGame in FishMiniGames){
            if(miniGame == miniGameManager.GetGame(StartGameIndex)){
                perRendererWater.darkControl = 1;
                perRendererWater.normalScale = 0.02f;

                return;
            }
        }
        foreach(var miniGame in SkyMiniGames){
            if(miniGame == miniGameManager.GetGame(StartGameIndex)){
                perRendererWater.darkControl = 1;
                perRendererWater.normalScale = 0f;

                return;
            }
        }
        perRendererWater.darkControl = 0;
        perRendererWater.normalScale = 0;
    }
#if UNITY_EDITOR
    [ContextMenu("Match Scene To Start Index")]
    public void Editor_MatchSceneToStartGameIndex(){
        for(int i=0; i<sharingSets.Length; i++){
            Undo.RecordObject(sharingSets[i].GameSets, "Change Activation");
            if(sharingSets[i].MatchIndex(StartGameIndex))
                sharingSets[i].GameSets.SetActive(true);
            else
                sharingSets[i].GameSets.SetActive(false);
            EditorUtility.SetDirty(sharingSets[i].GameSets);
        }
    //Tell Mini Game Manager to prepare the mini game to match the start up scene
        miniGameManager.Editor_ClearAllGame();
        miniGameManager.Editor_MatchSceneToStartGameIndex(StartGameIndex);
        if(includeLast) miniGameManager.Editor_MatchSceneToStartGameIndex(StartGameIndex - 1);
    Undo.RecordObject(perRendererWater, "Adjust Water Settings");
        PerRendererWater_Initiation();
    EditorUtility.SetDirty(perRendererWater);
    }
#endif
}
