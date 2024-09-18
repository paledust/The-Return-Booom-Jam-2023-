using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameBasic : MonoBehaviour
{
    [SerializeField] private bool autoControlGameAsset = true;
    [SerializeField] private GameObject miniGameAssetGroup;
    public bool IsPlaying{get{return isPlaying;}}
    private bool isPlaying = false;
    void Awake(){
        if(miniGameAssetGroup!=null)miniGameAssetGroup.SetActive(false);
    }
    public void EnterMiniGame(){
        isPlaying = true;
        EventHandler.OnKeyPressed    += OnKeyPressed;
        EventHandler.OnKeyReleased   += OnKeyReleased;
        EventHandler.OnAnyKeyPressed += OnAnyKeyPress;
        EventHandler.OnNoKeyPressed  += OnNoKeyPress;
        if(autoControlGameAsset && miniGameAssetGroup!=null) miniGameAssetGroup.SetActive(true);
        Initialize();
    }
    public void ExitMiniGame(){
        CleanUp();
        EventHandler.OnKeyPressed    -= OnKeyPressed;
        EventHandler.OnKeyReleased   -= OnKeyReleased;
        EventHandler.OnAnyKeyPressed -= OnAnyKeyPress;
        EventHandler.OnNoKeyPressed  -= OnNoKeyPress;

        isPlaying = false;
    }
    protected virtual void OnKeyPressed(UnityEngine.InputSystem.Key keyPressed){}
    protected virtual void OnKeyReleased(UnityEngine.InputSystem.Key keyReleased){}
    protected virtual void OnAnyKeyPress(){}
    protected virtual void OnNoKeyPress(){}   
    protected virtual void Initialize(){}
    protected virtual void CleanUp(){}
}
