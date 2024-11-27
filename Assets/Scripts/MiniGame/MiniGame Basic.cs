using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameBasic : MonoBehaviour
{
    [SerializeField] private GameObject miniGameAssetGroup;
    public bool IsPlaying{get{return isPlaying;}}
    private bool isPlaying = false;
    void Awake(){
        if(miniGameAssetGroup!=null) miniGameAssetGroup.SetActive(false);
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
        EventHandler.E_OnKeyPressed    += OnKeyPressed;
        EventHandler.E_OnKeyReleased   += OnKeyReleased;
        EventHandler.E_OnAnyKeyPressed += OnAnyKeyPress;
        EventHandler.E_OnNoKeyPressed  += OnNoKeyPress;
        if(miniGameAssetGroup!=null) miniGameAssetGroup.SetActive(true);
        Initialize();
    }
    public void ExitMiniGame(){
        CleanUp();
        EventHandler.E_OnKeyPressed    -= OnKeyPressed;
        EventHandler.E_OnKeyReleased   -= OnKeyReleased;
        EventHandler.E_OnAnyKeyPressed -= OnAnyKeyPress;
        EventHandler.E_OnNoKeyPressed  -= OnNoKeyPress;

        isPlaying = false;
    }
    public void UnloadMiniGame(){
        if(miniGameAssetGroup!=null) miniGameAssetGroup.SetActive(false);
        UnloadAssets();
    }
    protected virtual void UnloadAssets(){}
    protected virtual void OnKeyPressed(UnityEngine.InputSystem.Key keyPressed){}
    protected virtual void OnKeyReleased(UnityEngine.InputSystem.Key keyReleased){}
    protected virtual void OnAnyKeyPress(){}
    protected virtual void OnNoKeyPress(){}   
    protected virtual void Initialize(){}
    protected virtual void CleanUp(){}
}
