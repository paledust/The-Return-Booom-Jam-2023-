using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanSquareUnit : MonoBehaviour
{
    private enum SQUARE_STATE{Pending, Scanning, Error, Scanned}
    [SerializeField, ShowOnly] private SQUARE_STATE state = SQUARE_STATE.Pending;
    [SerializeField] private Projector m_projector;
    public bool Scanned{get{return state == SQUARE_STATE.Scanned;}}
    private DetectSeedMiniGame seedMiniGame;
    private string charge_anime_name = "Square_Charging";
    private string error_anime_name = "Square_Error";
    private string scanned_anime_name = "Square_Scanned";
    private string found_anime_name = "Square_Found";
    private Material projector_mat;
    public void Init(Material m_mat, DetectSeedMiniGame miniGame){
        seedMiniGame = miniGame;

        m_projector.material = projector_mat = Instantiate(m_mat);
        m_projector.material.color = miniGame.BlinkColor;
        m_projector.enabled = false;

        state = SQUARE_STATE.Pending;
    }
    public void CleanUp(){
        if(projector_mat!=null) Destroy(projector_mat);
    }
    public void StartScan(){
        if(state != SQUARE_STATE.Pending) return;
        m_projector.enabled = true;
        StopAllCoroutines();
        StartCoroutine(coroutineStartScan());
    }
    public void AbortScan(){
        StopAllCoroutines();
        StartCoroutine(coroutineAbortScan());
    }
    IEnumerator coroutineStartScan(){
        state = SQUARE_STATE.Scanning;
        Color targetColor = seedMiniGame.BlinkColor;
        Color initColor = targetColor;
        initColor.a = 0f;
        float duration = 2f;
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            float freq = Mathf.Lerp(10f, 25f, EasingFunc.Easing.QuadEaseIn(t));
            projector_mat.color = Color.Lerp(initColor, targetColor, 0.5f+0.5f*Mathf.Sin(t*Mathf.PI*freq));
            yield return null;
        }
        seedMiniGame.RefreshScan();
        StartCoroutine(coroutineShowScanResult(seedMiniGame.GetResult(this)));
    }
    IEnumerator coroutineShowScanResult(bool found){
        state = SQUARE_STATE.Scanned;
        Color targetColor = found?seedMiniGame.BingoColor:seedMiniGame.PassColor;
        Color initColor = targetColor;
        initColor *= 4f;
        initColor.a = targetColor.a;
        
        float duration = 2f;
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            projector_mat.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        projector_mat.color = targetColor;
    }
    IEnumerator coroutineAbortScan(){
        state = SQUARE_STATE.Error;
        Color targetColor = seedMiniGame.ErrorColor;
        Color initColor = targetColor;
        initColor.a = 0f;
        float duration = 0.75f;
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            projector_mat.color = Color.Lerp(initColor, targetColor, 0.5f+0.5f*Mathf.Sin(t*Mathf.PI*10f));
            yield return null;
        }
        projector_mat.color = initColor;
        m_projector.enabled = false;
        state = SQUARE_STATE.Pending;
    }
}
