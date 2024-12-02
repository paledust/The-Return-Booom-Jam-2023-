using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class ScanSquareUnit : MonoBehaviour
{
    private enum SQUARE_STATE{Pending, Scanning, Error, Scanned}
    [SerializeField, ShowOnly] private SQUARE_STATE state = SQUARE_STATE.Pending;
    [SerializeField] private Projector m_projector;
    private AudioSource m_audio;
    public bool IsScanned{get{return state == SQUARE_STATE.Scanned;}}
    public bool IsScanning{get{return state == SQUARE_STATE.Scanning;}}
    public bool IsPending{get{return state == SQUARE_STATE.Pending;}}
    private DetectSeedMiniGame seedMiniGame;
    private Material projector_mat;
    void Awake()=>m_audio = GetComponent<AudioSource>();
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
        m_projector.enabled = true;
        StopAllCoroutines();
        StartCoroutine(coroutineStartScan());
    }
    public void AbortScan(){
        StopAllCoroutines();
        StartCoroutine(coroutineAbortScan());
    }
    public void TurnOffScan()=>StartCoroutine(coroutineTurnOffScan());
    public void PlaySFX(AudioClip clip){
        m_audio.pitch = Random.Range(0.98f,1.02f);
        m_audio.PlayOneShot(clip);
    }
    public void PlaySFX(string clip){
        m_audio.pitch = Random.Range(0.98f, 1.02f);
        AudioManager.Instance.PlaySoundEffect(m_audio, clip, 1);
    }
    public void StopSFX()=>m_audio.Stop();
    IEnumerator coroutineTurnOffScan(){
        Color initColor, targetColor;
        initColor = targetColor = m_projector.material.color;
        targetColor.a = 0;

        float duration = 0.5f;
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            projector_mat.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        projector_mat.color = targetColor;
        m_projector.enabled = false;
    }
    IEnumerator coroutineStartScan(){
        state = SQUARE_STATE.Scanning;
        Color targetColor = seedMiniGame.BlinkColor;
        Color initColor = targetColor;
        initColor.a = 0f;
        float duration = 0.75f;
        
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            float freq = Mathf.Lerp(6.25f, 12.5f, EasingFunc.Easing.QuadEaseOut(t));
            m_projector.orthographicSize = Mathf.Lerp(0.1f, 0.5f, EasingFunc.Easing.Linear(t));
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
        Color initColor = seedMiniGame.ErrorColor;
        targetColor.a = 0;
        float duration = 0.5f;
        for(float t=0; t<1; t+=Time.deltaTime/duration){
            projector_mat.color = Color.Lerp(initColor, targetColor, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        projector_mat.color = targetColor;
        m_projector.enabled = false;
        state = SQUARE_STATE.Pending;
    }
}
