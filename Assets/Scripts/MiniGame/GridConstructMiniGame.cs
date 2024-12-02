using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class GridConstructMiniGame : MiniGameBasic
{
    private enum STAGE{Charging, Building}
[Header("Control")]
    [SerializeField] private float gridDelay = 1;
    [SerializeField] private float typingWindow = 0.5f;
    [SerializeField] private Animation m_gridBuildAnime;
    [SerializeField] private ParticleSystem m_projectorParticles;
[Header("Audio Source")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private string keyClips;
    [SerializeField] private AudioSource pc_loop;
    [SerializeField] private AudioSource scan_loop;
[Header("End MiniGame")]
    [SerializeField] private PlayableDirector end_director;
    private string clipName = string.Empty;
[Header("Information")]
    [SerializeField, ShowOnly] private STAGE stage = STAGE.Charging;
    [SerializeField, ShowOnly] private float gridChargeTime = 0;
    [SerializeField, ShowOnly] private float typingTestTime = 0;
    [SerializeField, ShowOnly] private bool isTyping = false;
    
    void Update(){
    //Typing Test
        if(isTyping){
            typingTestTime += Time.deltaTime;
            if(typingTestTime>=typingWindow){
                isTyping = false;
            }
        }

        switch(stage){
            case STAGE.Charging:
                if(isTyping){
                    gridChargeTime += Time.deltaTime;
                }
                if(gridChargeTime >= gridDelay){
                    stage = STAGE.Building;
                    m_projectorParticles.Play(true);
                    scan_loop.Play();
                }
                break;
            case STAGE.Building:
                if(isTyping){
                    m_gridBuildAnime[clipName].speed = Mathf.Lerp(m_gridBuildAnime[clipName].speed, 0.4f, Time.deltaTime * 5);
                    scan_loop.volume = Mathf.Lerp(scan_loop.volume, 1f, Time.deltaTime * 5);
                }
                else{
                    m_gridBuildAnime[clipName].speed = Mathf.Lerp(m_gridBuildAnime[clipName].speed, -0.02f, Time.deltaTime * 5);
                    scan_loop.volume = Mathf.Lerp(scan_loop.volume, 0f, Time.deltaTime * 5);
                }
                if(m_gridBuildAnime[clipName].normalizedTime >= 0.95f){
                    EventHandler.Call_OnEndMiniGame(this);
                    end_director.Play();
                    this.enabled = false;
                }
                break;
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
        this.enabled = true;

        stage = STAGE.Charging;
        gridChargeTime = 0;
        typingTestTime = 0;
        isTyping = false;
        
        clipName = m_gridBuildAnime.clip.name;
        m_gridBuildAnime[clipName].speed = 0;
        m_gridBuildAnime.Play();

        pc_loop.Play();
        StartCoroutine(coroutineFadePCLoop(true));
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        if(m_projectorParticles!=null)m_projectorParticles.Stop(true);
        scan_loop.Stop();
        this.enabled = false;
    }
    protected override void OnAnyKeyPress()
    {
        base.OnAnyKeyPress();

        if(stage!=STAGE.Building) return;

        m_projectorParticles.Play(true);
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();
        
        if(stage!=STAGE.Building) return;

        m_projectorParticles.Stop(true);
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);
        AudioManager.Instance.PlaySoundEffect(sfx_audio, keyClips, 1);

        isTyping = true;
        typingTestTime = 0;
    }
    public void FadeOutPCLoop()=>StartCoroutine(coroutineFadePCLoop(false));
    IEnumerator coroutineFadePCLoop(bool isFadeIn){
        float initVolume = pc_loop.volume;
        float targetVolume = isFadeIn?0.5f:0;
        for(float t=0; t<1; t+=Time.deltaTime*0.5f){
            pc_loop.volume = Mathf.Lerp(initVolume, targetVolume, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        pc_loop.volume = targetVolume;
        if(!isFadeIn) pc_loop.Stop();
    }
}
