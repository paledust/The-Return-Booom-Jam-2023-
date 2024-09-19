using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameBasic : MonoBehaviour
{
    [SerializeField] private bool autoActivateGameAsset = true;
    [SerializeField] private GameObject miniGameAssetGroup;
    public bool IsPlaying{get{return isPlaying;}}
    private bool isPlaying = false;
    void Awake(){
        if(miniGameAssetGroup!=null)miniGameAssetGroup.SetActive(false);
    }
#if UNITY_EDITOR
    public void Editor_PrepareMiniGame(bool isOn){
        if(miniGameAssetGroup!=null){
            if(isOn) miniGameAssetGroup.SetActive(true);
            else miniGameAssetGroup.SetActive(false);
        }
    }
#endif
    public void EnterMiniGame(){
        isPlaying = true;
        EventHandler.OnKeyPressed    += OnKeyPressed;
        EventHandler.OnKeyReleased   += OnKeyReleased;
        EventHandler.OnAnyKeyPressed += OnAnyKeyPress;
        EventHandler.OnNoKeyPressed  += OnNoKeyPress;
        if(autoActivateGameAsset && miniGameAssetGroup!=null) miniGameAssetGroup.SetActive(true);
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
