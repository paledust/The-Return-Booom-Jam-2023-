using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private MiniGameBasic[] miniGames;
[Header("Debug"), Space(10)]
    [SerializeField] private int startMiniGame;
    private int currentIndex = 0;
    void Awake(){
        for(int i=0; i<miniGames.Length; i++){
            miniGames[i].enabled = false;
        }
    }
    void OnEnable(){
    #if UNITY_EDITOR
        currentIndex = startMiniGame;
        miniGames[startMiniGame].EnterMiniGame();
    #else
        miniGames[0].EnterMiniGame();
    #endif
        EventHandler.OnEndMiniGame  += EndMiniGame;
        EventHandler.OnNextMiniGame += NextMiniGame;
    }
    void OnDisable(){
        for(int i=0; i<miniGames.Length; i++){
            if(miniGames[i].IsPlaying){
                miniGames[i].ExitMiniGame();
            }
        }
        EventHandler.OnEndMiniGame  -= EndMiniGame;
        EventHandler.OnNextMiniGame -= NextMiniGame;
    }
    void EndMiniGame(MiniGameBasic miniGame){
        Debug.Log("End MiniGame");
        miniGame.ExitMiniGame();
    }
    void NextMiniGame(){
        currentIndex ++;
        if(currentIndex<miniGames.Length) miniGames[currentIndex].EnterMiniGame();
        else Debug.LogError("Excceed the index of MiniGame");
    }
}
