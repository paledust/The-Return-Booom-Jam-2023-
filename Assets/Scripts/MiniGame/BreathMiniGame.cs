using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using SimpleAudioSystem;

public class BreathMiniGame : MiniGameBasic
{
    private enum BREATH_STATE{Idle, BreathingIn, BreathingOut}
    private enum INTERACTION_STAGE{Blur, Zoom}
    [SerializeField] private ParticleSystem fog_particle;
[Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vc_cam;
    [SerializeField] private float maxZoomInSpeed;
    [SerializeField] private float minZoomInSpeed;
[Header("Blur PP")]
    [SerializeField] private PostProcessVolume blur_pp;
    [SerializeField] private float maxBlurSpeed;
[Header("Breath Audio")]
    [SerializeField] private float breathInVolumeScale = 0.2f;
    [SerializeField] private float breathOutVolumeScale = 1f;
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip clockClip;
    [SerializeField] private string breathInClips;
    [SerializeField] private string breathOutClips;
[Header("Control")]
    [SerializeField] private float maxHoldTime = 2f;
[Header("Info")]
    [SerializeField, ShowOnly] private BREATH_STATE breathState = BREATH_STATE.Idle;
    [SerializeField, ShowOnly] private INTERACTION_STAGE interStage = INTERACTION_STAGE.Blur;
[Header("Timeline")]
    [SerializeField] private PlayableDirector m_endDirector;
    [SerializeField] private float directorPlayFOV = 50;
    IEnumerator coroutineBreath;
    IEnumerator coroutineCam;
    private float breathTimer = 0;
    private float camZoomSpeed = 0;
    private float ppFadeSpeed = 0;
    private int breathIndex = 0;
    private bool isEnding = false;
    void Update(){
        switch (interStage){
            case INTERACTION_STAGE.Blur:
                blur_pp.weight -= ppFadeSpeed * Time.deltaTime;
                blur_pp.weight = Mathf.Max(0, blur_pp.weight);
                vc_cam.m_Lens.FieldOfView -= camZoomSpeed * Time.deltaTime;
                if(blur_pp.weight == 0){
                    interStage = INTERACTION_STAGE.Zoom;
                }
                break;
            case INTERACTION_STAGE.Zoom:
                vc_cam.m_Lens.FieldOfView -= camZoomSpeed * Time.deltaTime;
                if(vc_cam.m_Lens.FieldOfView<directorPlayFOV && !isEnding){
                    isEnding = true;
                    fog_particle.Stop();
                    StartCoroutine(coroutineEndGame());
                }
                break;
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
        startColor.a *= 0.75f;
        mainModule.startColor = startColor;
        if(coroutineBreath!=null) StopCoroutine(coroutineBreath);
        StartCoroutine(coroutineBreath = coroutineBreathOut());

        if(coroutineCam!=null) StopCoroutine(coroutineCam);
        StartCoroutine(coroutineCam = coroutineChangeCamAndPP(false));
    }
    void breathIn(){
        breathState = BREATH_STATE.BreathingIn;
        AudioManager.Instance.PlaySoundEffect(sfx_audio, breathInClips, breathInVolumeScale);
        if(coroutineCam!=null) StopCoroutine(coroutineCam);
        StartCoroutine(coroutineCam = coroutineChangeCamAndPP(true));
    }
    protected override void Initialize()
    {
        base.Initialize();

        this.enabled = true;
        fog_particle.gameObject.SetActive(true);
        fog_particle.Play();
        camZoomSpeed = 0;
        ppFadeSpeed  = 0;
        blur_pp.weight = 0.5f;
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
        var clip = AudioManager.Instance.PlaySoundEffect(sfx_audio, breathOutClips, breathOutVolumeScale);
        yield return new WaitForSeconds(clip.length);
        breathState = BREATH_STATE.Idle;
    }
    IEnumerator coroutineChangeCamAndPP(bool isFadeIn){
        float initCamSpeed, initPPSpeed, targetCamSpeed, targetPPSpeed;
        float speed = isFadeIn?1:0.3f;
        initCamSpeed = isFadeIn?0:camZoomSpeed;
        initPPSpeed  = isFadeIn?0:ppFadeSpeed;

        float zoomInSpeed = (interStage==INTERACTION_STAGE.Blur)?minZoomInSpeed:maxZoomInSpeed;
        targetCamSpeed = isFadeIn?zoomInSpeed:0;
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
        var tempAudio = Instantiate(sfx_audio);
        tempAudio.PlayOneShot(clockClip);
        yield return new WaitForSeconds(clockClip.length);
        Destroy(tempAudio);
        EventHandler.Call_OnEndMiniGame(this);
        yield return coroutineChangeCamAndPP(false);
        m_endDirector.Play();
        this.enabled = false;
    }
}
