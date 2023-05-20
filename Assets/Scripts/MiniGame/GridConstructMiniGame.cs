using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridConstructMiniGame : MiniGameBasic
{
    private enum STAGE{Charging, Building}
    [SerializeField, ShowOnly] private STAGE stage;
[Header("Control")]
    [SerializeField] private float gridDelay = 1;
    [SerializeField] private Animation m_gridBuildAnime;
    [SerializeField] private ParticleSystem m_projectorParticles;
    [SerializeField] private Projector m_projector;
[Header("Audio Source")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip[] keyClips;
    private int keyClipPlayed = 0;
    private string clipName = string.Empty;
    private float gridChargeTime;
    void Update(){
        switch(stage){
            case STAGE.Charging:
                if(GameInputManager.Instance.HasKeyPressed){
                    gridChargeTime += Time.deltaTime;
                }
                if(gridChargeTime >= gridDelay){
                    stage = STAGE.Building;
                    m_projectorParticles.Play(true);
                }
                break;
            case STAGE.Building:
                if(GameInputManager.Instance.HasKeyPressed){
                    m_gridBuildAnime[clipName].speed = Mathf.Lerp(m_gridBuildAnime[clipName].speed, 0.4f, Time.deltaTime * 5);
                }
                else{
                    m_gridBuildAnime[clipName].speed = Mathf.Lerp(m_gridBuildAnime[clipName].speed, -0.02f, Time.deltaTime * 5);
                }
                if(m_gridBuildAnime[clipName].normalizedTime >= 0.95f){
                    EventHandler.Call_OnEndMiniGame(this);
                    this.enabled = false;
                }
                break;
        }
    }
    protected override void Initialize()
    {
        base.Initialize();

        clipName = m_gridBuildAnime.clip.name;
        m_gridBuildAnime[clipName].speed = 0;
        m_gridBuildAnime.Play();
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        m_projectorParticles.Stop(true);
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
        sfx_audio.PlayOneShot(keyClips[keyClipPlayed]);

        keyClipPlayed ++;
        if(keyClipPlayed>=keyClips.Length){
            keyClipPlayed = 0;
            Service.Shuffle<AudioClip>(ref keyClips);
        }
    }
}
