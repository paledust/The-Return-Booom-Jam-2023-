using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class BreathMiniGame : MiniGameBasic
{
    private enum BREATH_STATE{Idle, BreathingIn, BreathingOut}
    [SerializeField] private PostProcessVolume blur_pp;
    [SerializeField] private ParticleSystem fog_particle;
    [SerializeField] private Camera mainCam;
[Header("Breath Audio")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip[] breathInClips;
    [SerializeField] private AudioClip[] breathOutClips;
[Header("Control")]
    [SerializeField] private float maxHoldTime = 2f;
    [SerializeField, ShowOnly] private BREATH_STATE breathState = BREATH_STATE.Idle;
    private float breathTimer = 0;
    private int breathIndex = 0;
    void Update(){
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
        StartCoroutine(coroutineBreathOut());
    }
    void breathIn(){
        breathState = BREATH_STATE.BreathingIn;
        sfx_audio.PlayOneShot(breathInClips[breathIndex]);
    }
    protected override void Initialize()
    {
        base.Initialize();

        this.enabled = true;
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
        sfx_audio.PlayOneShot(clip);
        breathIndex ++;
        if(breathIndex == breathOutClips.Length){
            breathIndex = 0;
            Service.Shuffle<AudioClip>(ref breathOutClips);
            Service.Shuffle<AudioClip>(ref breathInClips);
        }
        yield return new WaitForSeconds(clip.length);
        breathState = BREATH_STATE.Idle;
    }
}
