using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class GuitarMiniGame : MiniGameBasic
{
    [SerializeField] private KeyMatrix_SO keyMatrix_SO;
[Header("Audio")]
    [SerializeField] private AudioSource guitarSource;
    [SerializeField, Range(0,1)] private float volumeScale;
    [SerializeField] private AudioClip[] D_dim_clips;
    [SerializeField] private AudioClip[] D_maj_clips;
    [SerializeField] private AudioClip[] Dm_clips;
    [SerializeField] private AudioClip[] D_clips;
[Header("Animation")]
    [SerializeField] private Animator m_characterAnime;
    [SerializeField] private float delayExit = 2f;
[Header("End")]
    [SerializeField] private float duration = 15f;
    [SerializeField] private float end_duration = 4f;
    [SerializeField] private PlayableDirector m_director; 
    private string playingGuitarBool = "IsPlaying";
[Header("Information")]
    [SerializeField, ShowOnly] private float playingTime;
    [SerializeField, ShowOnly] private float playingDuration = 0;
    [SerializeField, ShowOnly] private float afkDuration = 0;
    [SerializeField, ShowOnly] private bool playingGuitar = false;
    void Update(){
    //Playing Time Check
        if(playingTime > 0){
            playingTime -= Time.deltaTime;
            playingTime = Mathf.Max(0, playingTime);

            if(playingTime == 0 && playingGuitar){
                playingGuitar = false;
                m_characterAnime.SetBool(playingGuitarBool, playingGuitar);
            }
        }
    //Playing Guitar
        if(playingGuitar){
            if(playingDuration<duration) playingDuration += Time.deltaTime;
        }
        else{
            if(playingDuration>=duration) afkDuration += Time.deltaTime;
        }

        if(afkDuration >= end_duration){
            m_director.Play();
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
        this.enabled = true;
        
        playingTime = 0;
        playingDuration = 0;
        playingGuitar = false;
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        this.enabled = false;
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        playingTime = delayExit;
        afkDuration = 0;
        if(!playingGuitar){
            playingGuitar = true;
            m_characterAnime.SetBool(playingGuitarBool, playingGuitar);
        }

        Vector2Int coordinate = keyMatrix_SO.GetCoordinateFromKey(keyPressed);
        coordinate.x = coordinate.x%7;
        switch(coordinate.y){
            case 0:
                guitarSource.PlayOneShot(D_dim_clips[coordinate.x], volumeScale);
                break;
            case 1:
                guitarSource.PlayOneShot(D_maj_clips[coordinate.x], volumeScale);
                break;
            case 2:
                guitarSource.PlayOneShot(Dm_clips[coordinate.x], volumeScale);
                break;
            case 3:
                guitarSource.PlayOneShot(D_clips[coordinate.x], volumeScale);
                break;
        }
    }
}
