using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class WindyGrassMiniGame : MiniGameBasic
{
[Header("Interaction")]
    [SerializeField] private GameObject grassObject;
    [SerializeField] private KeyMatrix_SO keyMatrix_SO;
    [SerializeField] private Rect windArea;
[Header("Interaction End")]
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private float startTimelineDuration = 3;
[Header("Camera Render")]
    [SerializeField] private Camera RT_Camera;
    [SerializeField] private ParticleSystem m_particle;
    private Vector2[] spawnPos;
    private bool[] spawnTrigger;
    private ParticleSystem.EmitParams emitParams;
    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    private bool startCounting = false;
    private bool timelinePlaying = false;
    private float interaction_timer = 0;
    protected override void Initialize()
    {
        base.Initialize();
        grassObject.SetActive(true);
        this.enabled = true;

        RT_Camera.gameObject.SetActive(true);
        RT_Camera.enabled = true;

        spawnPos = new Vector2[ROLL*LINE];
        spawnTrigger = new bool[ROLL*LINE];

        emitParams.applyShapeToPosition = true;
        startCounting = false;

        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*windArea.width, -y/(ROLL-1.0f)*windArea.height)+new Vector2(-0.5f*windArea.width,0.5f*windArea.height);
            }
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        this.enabled = false;
        spawnPos = null;
        if(m_particle!=null)m_particle.Stop();
    }
    void Update(){
        if(startCounting && !timelinePlaying){
            if(Time.time > interaction_timer+startTimelineDuration){
                timelinePlaying = true;
                m_director.Play();
            }
        }
    }
    void FixedUpdate(){
        Vector3 location;
        for(int i=0; i<ROLL*LINE; i++){
            if(spawnTrigger[i]){
                location.x = spawnPos[i].x;
                location.z = spawnPos[i].y;
                location.y = m_particle.transform.position.y;
                emitParams.position = location;
                m_particle.Emit(emitParams, 1);
            }
        }
    }

    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        if(!startCounting){
            startCounting = true;
            interaction_timer = Time.time;
        }
        Vector2Int coordinate = keyMatrix_SO.GetCoordinateFromKey(keyPressed);
        spawnTrigger[coordinate.y*LINE+coordinate.x] = true;
    }
    protected override void OnKeyReleased(Key keyReleased)
    {
        base.OnKeyReleased(keyReleased);

        Vector2Int coordinate = keyMatrix_SO.GetCoordinateFromKey(keyReleased);
        spawnTrigger[coordinate.y*LINE+coordinate.x] = false;
    }
    public void EndWindyGrassMiniGame(){
        EventHandler.Call_OnEndMiniGame(this);
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(windArea.xMin, 0, windArea.yMin);
        Vector3 size= new Vector3(windArea.size.x, 0, windArea.size.y);
        Gizmos.DrawCube(center, size);
    }
}
