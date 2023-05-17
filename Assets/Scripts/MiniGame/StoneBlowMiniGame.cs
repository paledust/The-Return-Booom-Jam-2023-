using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlowMiniGame : MiniGameBasic
{
    [SerializeField] private List<ConstantForce> stoneDebris;
    [SerializeField] private Vector2 torqueRange;
    [SerializeField] private Vector2 forceRange;
    [SerializeField] private Vector2 timeRange;
[Header("Sand Particles")]
    [SerializeField] private ParticleSystem sandParticles;
    private float totalCount;
    protected override void Initialize(){
        base.Initialize();
        totalCount = stoneDebris.Count;
    }
    protected override void OnKeyPressed(UnityEngine.InputSystem.Key keyPressed){
        int index = Random.Range(0, stoneDebris.Count);
        ConstantForce debri = stoneDebris[index];

        stoneDebris.RemoveAt(index);
        StartCoroutine(coroutineForceOnRock(debri, Mathf.Lerp(timeRange.x, timeRange.y, 1-stoneDebris.Count/(totalCount-1))));

        if(stoneDebris.Count == 0){
            sandParticles.Play(true);
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    IEnumerator coroutineForceOnRock(ConstantForce cforce, float time){
        for(float t=0; t<1; t+=Time.deltaTime/time){
            cforce.torque = Vector3.right * Mathf.Lerp(torqueRange.x, torqueRange.y, t);
            cforce.force  = Vector3.forward * Mathf.Lerp(forceRange.x, forceRange.y, t);
            yield return null;
        }
    }
}
