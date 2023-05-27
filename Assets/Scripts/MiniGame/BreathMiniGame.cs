using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class BreathMiniGame : MiniGameBasic
{
    private enum BREATH_STATE{Idle, BreathingIn, BreathingOut}
    [SerializeField] private ParticleSystem fog_particle;
[Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vc_cam;
    [SerializeField] private float maxZoomInSpeed;
[Header("Blur PP")]
    [SerializeField] private PostProcessVolume blur_pp;
    [SerializeField] private float maxBlurSpeed;
[Header("Breath Audio")]
    [SerializeField] private float breathInVolumeScale = 0.2f;
    [SerializeField] private float breathOutVolumeScale = 1f;
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip[] breathInClips;
    [SerializeField] private AudioClip[] breathOutClips;
[Header("Control")]
    [SerializeField] private float maxHoldTime = 2f;
    [SerializeField, ShowOnly] private BREATH_STATE breathState = BREATH_STATE.Idle;
[Header("Timeline")]
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private PlayableDirector m_endDirector;
    [SerializeField] private float directorPlayFOV = 50;
    private float breathTimer = 0;
    private float camZoomSpeed = 0;
    private float ppFadeSpeed = 0;
    private int breathIndex = 0;
    private bool directorPlayed = false;
    void Update(){
        vc_cam.m_Lens.FieldOfView -= camZoomSpeed * Time.deltaTime;
        blur_pp.weight -= ppFadeSpeed * Time.deltaTime;
        blur_pp.weight = Mathf.Max(0, blur_pp.weight);

        if(blur_pp.weight == 0){
            EventHandler.Call_OnEndMiniGame(this);
            StartCoroutine(coroutineEndGame());
        }
        if(vc_cam.m_Lens.FieldOfView < directorPlayFOV && !directorPlayed){
            directorPlayed = true;
            m_director.Play();
        }

        if(breathState == BREATH_STATE.BreathingIn){
            breathTimer += Time.deltaTime;
            if(breathTimer>=maxHoldTime){
                breathOut();
            }
        }
    }
    void breathOut(){
        breathState = BREATH_STATE.BreathingOut;
        sfx_audio.Stop();
        breathTimer = 0;
        var mainModule = fog_particle.main;
        Color startColor = mainModule.startColor.color;
        startColor.a *= 0.5f;
        mainModule.startColor = startColor;
        StopAllCoroutines();
        StartCoroutine(coroutineBreathOut());
        StartCoroutine(coroutineChangeCamAndPP(false));
    }
    void breathIn(){
        breathState = BREATH_STATE.BreathingIn;
        sfx_audio.PlayOneShot(breathInClips[breathIndex], breathInVolumeScale);
        StopAllCoroutines();
        StartCoroutine(coroutineChangeCamAndPP(true));
    }
    protected override void Initialize()
    {
        base.Initialize();

        this.enabled = true;
        camZoomSpeed = 0;
        ppFadeSpeed  = 0;
        blur_pp.weight = 1;
    }

    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        if(breathState == BREATH_STATE.Idle) breathIn();
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();
        if(breathState == BREATH_STATE.BreathingIn) breathOut();
    }
    IEnumerator coroutineBreathOut(){
        AudioClip clip = breathOutClips[breathIndex];
        sfx_audio.PlayOneShot(clip, breathOutVolumeScale);
        breathIndex ++;
        if(breathIndex == breathOutClips.Length){
            breathIndex = 0;
            Service.Shuffle<AudioClip>(ref breathOutClips);
            Service.Shuffle<AudioClip>(ref breathInClips);
        }
        yield return new WaitForSeconds(clip.length);
        breathState = BREATH_STATE.Idle;
    }
    IEnumerator coroutineChangeCamAndPP(bool isFadeIn){
        float initCamSpeed, initPPSpeed, targetCamSpeed, targetPPSpeed;
        float speed = isFadeIn?1:0.3f;
        initCamSpeed = isFadeIn?0:camZoomSpeed;
        initPPSpeed  = isFadeIn?0:ppFadeSpeed;

        targetCamSpeed = isFadeIn?maxZoomInSpeed:0;
        targetPPSpeed  = isFadeIn?maxBlurSpeed:0;

        for(float t=0; t<1; t+=Time.deltaTime*speed){
            camZoomSpeed = Mathf.Lerp(initCamSpeed, targetCamSpeed, EasingFunc.Easing.SmoothInOut(t));
            ppFadeSpeed = Mathf.Lerp(initPPSpeed, targetPPSpeed, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        camZoomSpeed = targetCamSpeed;
        ppFadeSpeed = targetPPSpeed;
    }
    IEnumerator coroutineEndGame(){
        yield return coroutineChangeCamAndPP(false);
        fog_particle.Stop();
        m_endDirector.Play();
        this.enabled = false;
    }
}
