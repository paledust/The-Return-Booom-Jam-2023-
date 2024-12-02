using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.Playables;

public class StoneBlowMiniGame : MiniGameBasic
{
    [SerializeField] private List<ConstantForce> stoneDebris;
    [SerializeField] private Vector2 torqueRange;
    [SerializeField] private Vector2 forceRange;
    [SerializeField] private Vector2 timeRange;
    [SerializeField] private Vector2 spawnIntersectionRange;
[Header("Other stone")]
    [SerializeField] private StoneDebrisGenerator stoneGenerator;
[Header("Audio Source")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioSource heli_loop;
    [SerializeField] private string rockClips;
    [SerializeField] private float volumeScale = 1;
    [SerializeField, Range(0,1)] private float maxVolume;
[Header("End MiniGame")]
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private float directorStartDelay;
    [SerializeField] private PlayableDirector director;
    private List<ConstantForce> touchedStoneDebris;
    private float totalCount;
    private float targetVolume = 0;

    void Update(){
        heli_loop.volume = Mathf.Lerp(heli_loop.volume, targetVolume, Time.deltaTime*5);
    }
    protected override void Initialize(){
        base.Initialize();
        this.enabled = true;
        heli_loop.volume = 0;
        heli_loop.Play();
        totalCount = stoneDebris.Count;
        touchedStoneDebris = new List<ConstantForce>();
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        this.enabled = false;
    }
    protected override void OnKeyPressed(UnityEngine.InputSystem.Key keyPressed){
        base.OnKeyPressed(keyPressed);
        
        float debrisProgress = 1-stoneDebris.Count/(totalCount-1);

        targetVolume = debrisProgress * maxVolume;

        if(!stoneGenerator.gameObject.activeSelf) stoneGenerator.gameObject.SetActive(true);
        stoneGenerator.spawnIntersection = Mathf.Lerp(spawnIntersectionRange.x, spawnIntersectionRange.y, debrisProgress);

        int index = Random.Range(0, stoneDebris.Count);
        ConstantForce debri = stoneDebris[index];

        touchedStoneDebris.Add(stoneDebris[index]);
        stoneDebris.RemoveAt(index);
        StartCoroutine(coroutineForceOnRock(debri, Mathf.Lerp(timeRange.x, timeRange.y, debrisProgress)));

        AudioManager.Instance.PlaySoundEffect(sfx_audio, rockClips, volumeScale);

        if(stoneDebris.Count == 0){
            sandParticles.Play(true);
            StartCoroutine(coroutineEndMiniGame());
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    public void StopSandStorm()=>sandParticles.Stop(true);
    public void StopStoneGenerate()=>stoneGenerator.StopStoneSpawn();
    public void FadeHeliLoop()=> StartCoroutine(coroutineFadeHeliLoop());
    IEnumerator coroutineFadeHeliLoop(){
        for(float t=0; t<1; t+=Time.deltaTime/2f){
            heli_loop.volume = Mathf.Lerp(1,0,EasingFunc.Easing.SmoothInOut(t));
            yield return null;
        }
        heli_loop.volume = 0;
        heli_loop.Stop();
    }
    IEnumerator coroutineForceOnRock(ConstantForce cforce, float time){
        for(float t=0; t<1; t+=Time.deltaTime/time){
            cforce.torque = Vector3.right * Mathf.Lerp(torqueRange.x, torqueRange.y, t);
            cforce.force  = Vector3.forward * Mathf.Lerp(forceRange.x, forceRange.y, t);
            yield return null;
        }
    }
    IEnumerator coroutineEndMiniGame(){
        yield return new WaitForSeconds(directorStartDelay);
        director.Play();
    }
}
