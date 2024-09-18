using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private MiniGameBasic[] miniGames;
    private int currentIndex = 0;
    void Awake(){
        for(int i=0; i<miniGames.Length; i++){
            miniGames[i].enabled = false;
        }
    }
    void OnEnable(){
        EventHandler.OnEndMiniGame  += EndMiniGame;
        EventHandler.OnNextMiniGame += NextMiniGame;
    }
    void OnDisable(){
        EventHandler.OnEndMiniGame  -= EndMiniGame;
        EventHandler.OnNextMiniGame -= NextMiniGame;
    }
    void EndMiniGame(MiniGameBasic miniGame){
        miniGame.ExitMiniGame();
    }
    void NextMiniGame(){
        currentIndex ++;
        if(currentIndex<miniGames.Length) miniGames[currentIndex].EnterMiniGame();
        else Debug.LogError("Excceed the index of MiniGame");
    }
    public void StartGame(int startIndex=0){
        currentIndex = startIndex;
        miniGames[startIndex].EnterMiniGame();
    }
    public void EndGame(){
        GameManager.Instance.EndGame();
    }
}
