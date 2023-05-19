using System.Collections;
using System.Collections.Generic;
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
[Header("End MiniGame")]
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private float directorStartDelay;
    [SerializeField] private PlayableDirector director;
    private float totalCount;
    protected override void Initialize(){
        base.Initialize();
        totalCount = stoneDebris.Count;
    }
    protected override void OnKeyPressed(UnityEngine.InputSystem.Key keyPressed){
        base.OnKeyPressed(keyPressed);
        
        float debrisProgress = 1-stoneDebris.Count/(totalCount-1);

        if(!stoneGenerator.gameObject.activeSelf) stoneGenerator.gameObject.SetActive(true);
        stoneGenerator.spawnIntersection = Mathf.Lerp(spawnIntersectionRange.x, spawnIntersectionRange.y, debrisProgress);

        int index = Random.Range(0, stoneDebris.Count);
        ConstantForce debri = stoneDebris[index];

        stoneDebris.RemoveAt(index);
        StartCoroutine(coroutineForceOnRock(debri, Mathf.Lerp(timeRange.x, timeRange.y, debrisProgress)));

        if(stoneDebris.Count == 0){
            sandParticles.Play(true);
            StartCoroutine(coroutineEndMiniGame());
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    public void StopSandStorm()=>sandParticles.Stop(true);
    public void StopStoneGenerate()=>stoneGenerator.StopStoneSpawn();
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
