using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class ThrowingStoneMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect stoneRect;
[Header("Throw Stones")]
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform throwStartPos;
    [SerializeField] private float throwStartOffset;
    [SerializeField] private Vector2 throwSpeedRange;
    [SerializeField] private Vector2 angularSpeedRange;

    private Vector2[] throwPos;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    protected override void Initialize()
    {
        base.Initialize();

        throwPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                throwPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*stoneRect.width, -y/(ROLL-1.0f)*stoneRect.height)+new Vector2(-0.5f*stoneRect.width,0.5f*stoneRect.height);
            }
        }
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
