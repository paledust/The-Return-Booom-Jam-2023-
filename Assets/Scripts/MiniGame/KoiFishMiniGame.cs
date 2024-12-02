using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class KoiFishMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect fishRect;
    [SerializeField] private ParticleSystem fishParticles;
    [SerializeField] private ParticleSystem rippleParticles;
    [SerializeField] private ParticleSystem fireBurstParticles;
    [SerializeField] private ParticleSystem fireParticles;
[Header("Progression")]
    [SerializeField] private float targetFishAmount = 50;
    [SerializeField] private AnimationCurve emitRate;
    [SerializeField] private AnimationCurve fireRadiusRange;
    [SerializeField] private AnimationCurve fireEmitRange;
[Header("End")]
    [SerializeField] private ParticleSystem P_fireBurst;
    [SerializeField] private PlayableDirector tl_giantFishOut;
    private ParticleSystem.EmissionModule fireEmitModule;
    private ParticleSystem.ShapeModule fireShapeModule;
    private Vector2[] spawnPos;
    private float progress = 0;
    private int spawnedFishAmount = 0;
    private float emitStep = 0.01f;
    private float lastEmitTime = 0;

    protected override void Initialize()
    {
        base.Initialize();

        spawnPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*fishRect.width, -y/(ROLL-1.0f)*fishRect.height)+new Vector2(-0.5f*fishRect.width,0.5f*fishRect.height);
            }
        }

        fireEmitModule  = fireParticles.emission;
        fireShapeModule = fireParticles.shape;

        this.enabled = true;
    }
    void Update(){
        float targetProgress = (0f+spawnedFishAmount)/(targetFishAmount+0f);
        if(progress != targetProgress)
            progress = Service.SmoothToValue(progress, targetProgress, Time.deltaTime*2, 0.01f);
        
        fireShapeModule.radius = fireRadiusRange.Evaluate(progress);
        fireEmitModule.rateOverTimeMultiplier =fireEmitRange.Evaluate(progress);

        if(progress >= 0.99f){
            EventHandler.Call_OnEndMiniGame(this);
            P_fireBurst.Play();
            tl_giantFishOut.Play();
            fireParticles.Stop();
        }
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);
        emitStep = Time.time;

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 location;
        int index = coordinate.y*LINE + coordinate.x;

        location = spawnPos[index];
        location.z = location.y;

        location.y = rippleParticles.transform.position.y;
        rippleParticles.transform.position = location;
        rippleParticles.Play(true);

        bool emitFlag = true;
        if(Time.time-lastEmitTime<emitStep && Random.value>emitRate.Evaluate(progress)){
            emitFlag = false;
        }
        if(emitFlag){
            location.y = fishParticles.transform.position.y;
            fishParticles.transform.position = location;
            fishParticles.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            fishParticles.Play(true);

            location.y = fireBurstParticles.transform.position.y;
            location.x = Mathf.Lerp(-2.47f, -3f, progress);
            fireBurstParticles.transform.position = location;
            fireBurstParticles.Play();

            lastEmitTime = Time.time;
            spawnedFishAmount ++;
        }
    }

    protected override void CleanUp()
    {
        base.CleanUp();
        this.enabled = false;
    }
}
