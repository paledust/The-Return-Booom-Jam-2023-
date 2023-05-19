using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridConstructMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private Animation m_gridBuildAnime;
    [SerializeField] private ParticleSystem m_projectorParticles;
    [SerializeField] private Projector m_projector;
[Header("Audio Source")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip[] keyClips;
    private int keyClipPlayed = 0;
    private string clipName = string.Empty;
    private IEnumerator FadeAnimeSpeed;
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

        m_projectorParticles.Play(true);

        if(FadeAnimeSpeed!=null) StopCoroutine(FadeAnimeSpeed);
        StartCoroutine(FadeAnimeSpeed = coroutineFadeAnimeSpeed(.4f));
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();

        m_projectorParticles.Stop(true);
        if(FadeAnimeSpeed!=null) StopCoroutine(FadeAnimeSpeed);
        StartCoroutine(FadeAnimeSpeed = coroutineFadeAnimeSpeed(0));
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

        Debug.Log(m_gridBuildAnime[clipName].normalizedTime);
        if(m_gridBuildAnime[clipName].normalizedTime >= 0.95f){
            Debug.Log("End");
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    IEnumerator coroutineFadeAnimeSpeed(float targetSpeed){
        float speed = m_gridBuildAnime[clipName].speed;

        for(float t=0; t<1; t+=Time.deltaTime*5){
            m_gridBuildAnime[clipName].speed = Mathf.Lerp(speed, targetSpeed, EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        m_gridBuildAnime[clipName].speed = targetSpeed;
    }
}
