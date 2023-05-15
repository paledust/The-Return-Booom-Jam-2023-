using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private MiniGameBasic[] miniGames;
    void OnEnable(){
        miniGames[0].EnterMiniGame();
        EventHandler.OnEndMiniGame += EndMiniGame;
    }
    void OnDisable(){
        for(int i=0; i<miniGames.Length; i++){
            if(miniGames[i].IsPlaying){
                miniGames[i].ExitMiniGame();
            }
        }
        EventHandler.OnEndMiniGame -= EndMiniGame;
    }
    void EndMiniGame(MiniGameBasic miniGame){
        Debug.Log("End MiniGame");
        miniGame.ExitMiniGame();
    }
}
