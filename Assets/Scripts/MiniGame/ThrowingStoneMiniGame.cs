using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class ThrowingStoneMiniGame : MiniGameBasic
{
[Header("Intro")]
    [SerializeField] private ParticleSystem P_fireBurst;
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect stoneRect;
[Header("Particles")]
    [SerializeField] private ParticleSystem p_ripple;
    [SerializeField] private ParticleSystem p_splash;
[Header("Water Info")]
    [SerializeField] private float waterHeight;
    [SerializeField] private float minimumDriftAngularSpeed;
    [SerializeField, Range(0, 1)] private float waterAngularFriction;
    [SerializeField, Range(0, 1)] private float waterSpeedFriction;
    [SerializeField, Range(0, 1)] private float kickUpFactor = 0.8f;
[Header("Throw Stones")]
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform throwStartPos;
    [SerializeField] private float throwStartOffset;
    [SerializeField] private Vector2 throwSpeedRange;
    [SerializeField] private Vector2 angularSpeedRange;

    private Vector2[] throwPos;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

    public static float WATER_ANGULAR_FRICTION;
    public static float WATER_SPEED_FRICTION;
    public static float WATER_HEIGHT = 2.19f;
    public static float MINIMUM_DRIFTANGULAR_SPEED = 200f;
    public static float KICK_UP_FACTOR = 0.8f;
    protected override void Initialize()
    {
        base.Initialize();

        WATER_HEIGHT = waterHeight;
        MINIMUM_DRIFTANGULAR_SPEED = minimumDriftAngularSpeed;
        WATER_ANGULAR_FRICTION = waterAngularFriction;
        WATER_SPEED_FRICTION = waterSpeedFriction;
        KICK_UP_FACTOR =  kickUpFactor;

        throwPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                throwPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*stoneRect.width, -y/(ROLL-1.0f)*stoneRect.height)+new Vector2(-0.5f*stoneRect.width,0.5f*stoneRect.height);
            }
        }

        EventHandler.E_OnStoneTouchWater += StoneHitWater;
    }
    protected override void CleanUp()
    {
        EventHandler.E_OnStoneTouchWater -= StoneHitWater;
    }
#if UNITY_EDITOR
    void OnValidate(){
        WATER_HEIGHT = waterHeight;
        MINIMUM_DRIFTANGULAR_SPEED = minimumDriftAngularSpeed;
        WATER_ANGULAR_FRICTION = waterAngularFriction;
        WATER_SPEED_FRICTION = waterSpeedFriction;
        KICK_UP_FACTOR =  kickUpFactor;
    }
#endif
    void StoneHitWater(Vector3 position){
        position.y = p_splash.transform.position.y;
        p_splash.transform.position = position;
        p_splash.Play();

        position.y = p_ripple.transform.position.y;
        p_ripple.transform.position = position;
        p_ripple.Play();
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 target;
        int index = coordinate.y*LINE + coordinate.x;
        target = throwPos[index];
        target.y = 2.19f;

    //Calculate Offseted Start Position
        Vector3 dir = target - throwStartPos.position;
        dir.y = 0;
        float _r = throwStartOffset / Vector3.Dot(dir, Vector3.forward);

        var stone = Instantiate(stonePrefab, transform).GetComponent<CobbleBehavior>();
        stone.transform.position = throwStartPos.position + dir*_r;
        stone.transform.rotation = Quaternion.Euler(0,Random.Range(0, 360),0);
        stone.ThrowCobble(dir.normalized * throwSpeedRange.GetRndValueInVector2Range(), angularSpeedRange.GetRndValueInVector2Range().GetNegOrPosFloat());
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(stoneRect.xMin, 2.2f, stoneRect.yMin);
        Vector3 size= new Vector3(stoneRect.size.x, 0, stoneRect.size.y);
        Gizmos.DrawCube(center, size);

        DebugExtension.DrawPoint(throwStartPos.position + Vector3.forward*throwStartOffset, Color.green, 1f);
    }
}
