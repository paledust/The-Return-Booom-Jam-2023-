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
    void Start()
    {
        for(int i=0; i<sharingSets.Length; i++){
            if(sharingSets[i].MatchIndex(StartGameIndex))
                sharingSets[i].GameSets.SetActive(true);
            else
                sharingSets[i].GameSets.SetActive(false);
        }
        miniGameManager.StartGame(StartGameIndex);
    }
#if UNITY_EDITOR
    [ContextMenu("Match Scene To Start Index")]
    public void MatchSceneToStartGameIndex(){
        for(int i=0; i<sharingSets.Length; i++){
            Undo.RecordObject(sharingSets[i].GameSets, "Change Activation");
            if(sharingSets[i].MatchIndex(StartGameIndex))
                sharingSets[i].GameSets.SetActive(true);
            else
                sharingSets[i].GameSets.SetActive(false);
            EditorUtility.SetDirty(sharingSets[i].GameSets);
        }
    //Tell Mini Game Manager to prepare the mini game to match the start up scene
    }
#endif
}
