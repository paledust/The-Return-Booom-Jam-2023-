using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameBasic : MonoBehaviour
{
    public bool IsPlaying{get{return isPlaying;}}
    private bool isPlaying = false;
    public void EnterMiniGame(){
        isPlaying = true;
        EventHandler.OnKeyPressed += OnKeyPressed;
        Initialize();
    }
    public void ExitMiniGame(){
        CleanUp();
        EventHandler.OnKeyPressed += OnKeyPressed;
        isPlaying = false;
    }
    protected abstract void OnKeyPressed(KeyCode keyPressed);
    protected virtual void Initialize(){}
    protected virtual void CleanUp(){}
}
