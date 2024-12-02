using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

using SimpleAudioSystem;

public class RainyMiniGame : MiniGameBasic
{
    [SerializeField] private KeyMatrix_SO keyMatrix_Data;
    [SerializeField] private Camera m_rt_cam;
    [SerializeField] private Rect rainArea;
[Header("Particle Control")]
    [SerializeField] private ParticleSystem m_rainParticles;
    [SerializeField] private int Cycle = 3;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float intervel = 0.01f;
    [SerializeField] private int Count = 1;
[Header("Audio")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private float sfx_step = 1.1f;
    [SerializeField] private string rainDropClips;
    [SerializeField] private string amb_rain_small_name;
    [SerializeField] private string amb_rain_mid_name;
    [SerializeField] private string amb_rain_heavy_name;
[Header("Rain Loop")]
    [SerializeField] private ParticleSystem m_rain_loop;
    [SerializeField] private float rain_chargeSpeed = 1f;
    [SerializeField] private float rain_dieSpeed = 0.25f;
[Header("End")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private float timeline_delay = 5;
    private Vector2[] dropPos;
    private ParticleSystem.EmissionModule emission;
[Header("Info")]
    [SerializeField, ShowOnly] private float rain_amount = 0;
    private float minimalRain_amount = 0;
    private bool isRaining = false;
    private const float SMALL_LEVEL = 0.25f;
    private const float MID_LEVEL = 0.5f;
    private const float HEAVY_LEVEL = 0.75f;
    private float sfx_time;

    void Update(){
        if(isRaining && rain_amount<1) rain_amount += rain_chargeSpeed * Time.deltaTime;
        if(!isRaining && rain_amount>minimalRain_amount) {
            rain_amount -= rain_dieSpeed * Time.deltaTime;
            rain_amount = Mathf.Max(rain_amount, minimalRain_amount);
        }

        if(rain_amount>=SMALL_LEVEL && rain_amount<MID_LEVEL && minimalRain_amount!=SMALL_LEVEL){
            minimalRain_amount = SMALL_LEVEL;
            m_rain_loop.Play(true);
            emission.rateOverTimeMultiplier = 10;
            AudioManager.Instance.PlayAmbience(amb_rain_small_name, true, 4f, .03f);
        }
        if(rain_amount>=MID_LEVEL && rain_amount<HEAVY_LEVEL && minimalRain_amount!=MID_LEVEL){
            minimalRain_amount = MID_LEVEL;
            emission.rateOverTimeMultiplier = 100;
            AudioManager.Instance.PlayAmbience(amb_rain_mid_name, true, 3f, .06f);
        }
        if(rain_amount>HEAVY_LEVEL && rain_amount<1 && minimalRain_amount!=HEAVY_LEVEL){
            minimalRain_amount = HEAVY_LEVEL;
            emission.rateOverTimeMultiplier = 200;
            AudioManager.Instance.PlayAmbience(amb_rain_heavy_name, true, 4f, .2f);
        }
        if(rain_amount >= 1){
            minimalRain_amount = 1;
            emission.rateOverTimeMultiplier = 500;
            StartCoroutine(CommonCoroutine.DelayAction(()=>director.Play(), timeline_delay));
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
        this.enabled = true;

        m_rt_cam.enabled = true;
        sfx_time = -sfx_step;

        emission = m_rain_loop.emission;
        emission.rateOverTimeMultiplier = 0;

        rain_amount = 0;
        dropPos = new Vector2[ROLL*LINE];
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                dropPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*rainArea.width, -y/(ROLL-1.0f)*rainArea.height)+new Vector2(-0.5f*rainArea.width,0.5f*rainArea.height);
            }
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        this.enabled = false;
        dropPos = null;
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();

        isRaining = false;
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        isRaining = true;
        Vector2Int coordinate = keyMatrix_Data.GetCoordinateFromKey(keyPressed);
        Vector3 location = dropPos[coordinate.x+coordinate.y*LINE];
        location.z = location.y;
        location.y = m_rainParticles.transform.position.y;

        if(Time.time > sfx_step+sfx_time){
            sfx_time = Time.time;
            StartCoroutine(CommonCoroutine.DelayAction(()=>AudioManager.Instance.PlaySoundEffect(sfx_audio, rainDropClips), 0.3f));
        }
        StartCoroutine(CoroutineBurstRainDrops(location));
    }
    public void DisableRT_Cam()=>m_rt_cam.enabled = false;

    IEnumerator CoroutineBurstRainDrops(Vector3 pos){
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for(int i=0; i<Cycle; i++){
            float Size = Random.Range(m_rainParticles.main.startSize.constantMin, m_rainParticles.main.startSize.constantMax);
            Vector2 rnd = Random.insideUnitCircle * radius;
            emitParams.startColor = m_rainParticles.main.startColor.color;
            emitParams.position = pos + new Vector3(rnd.x, 0, rnd.y);
            emitParams.startSize= Size;
            m_rainParticles.Emit(emitParams, Count);
            yield return new WaitForSeconds(intervel);
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(rainArea.xMin, 0, rainArea.yMin);
        Vector3 size= new Vector3(rainArea.size.x, 0, rainArea.size.y);
        Gizmos.DrawCube(center, size);
    }
}
