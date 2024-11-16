using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Main_Initiator : MonoBehaviour
{
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
    [SerializeField] private MiniGameManager miniGameManager;
[Header("Sharing Sets")]
    [SerializeField] private SharingSets[] sharingSets;
[Header("Special Sets")]
    [SerializeField] private PerRendererWater perRendererWater;
    [SerializeField] private MiniGameBasic[] FishMiniGames;
    void Start()
    {
        for(int i=0; i<sharingSets.Length; i++){
            if(sharingSets[i].MatchIndex(StartGameIndex))
                sharingSets[i].GameSets.SetActive(true);
            else
                sharingSets[i].GameSets.SetActive(false);
        }

        miniGameManager.StartGame(StartGameIndex);
        PerRendererWater_Initiation();
    }
    void PerRendererWater_Initiation(){
        foreach(var miniGame in FishMiniGames){
            if(miniGame == miniGameManager.GetGame(StartGameIndex)){
                perRendererWater.darkControl = 1;
                perRendererWater.normalScale = 0.02f;

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
        miniGameManager.Editor_MatchSceneToStartGameIndex(StartGameIndex);
    Undo.RecordObject(perRendererWater, "Adjust Water Settings");
        PerRendererWater_Initiation();
    EditorUtility.SetDirty(perRendererWater);
    }
#endif
}
