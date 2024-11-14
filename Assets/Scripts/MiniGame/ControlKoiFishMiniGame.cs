using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlKoiFishMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect fishRect;
[Header("Fish")]
    [SerializeField] private FishAI fish;
    [SerializeField] private Collider fishTrigger;
    [SerializeField] private Vector2 fishSpeedRange;
    [SerializeField] private Vector2 fishRotateSpeedRange;
[Header("VFX")]
    [SerializeField] private ParticleSystem p_ripple;
    [SerializeField] private float particleOffset = 0.7f;

    private Vector2[] guidePos;
    private CoroutineExcuter fishReleaser;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

    protected override void Initialize()
    {
        base.Initialize();

        guidePos = new Vector2[ROLL*LINE];
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                guidePos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*fishRect.width, -y/(ROLL-1.0f)*fishRect.height)+new Vector2(-0.5f*fishRect.width,0.5f*fishRect.height);
            }
        }

        fishReleaser = new CoroutineExcuter(this);
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 target;
        target = guidePos[coordinate.y*LINE + coordinate.x];
        target.z = target.y;
        target.y = 0;

        fish.AssignTarget(target);
        fish.TransitionMovement(fishSpeedRange.y, fishRotateSpeedRange.y, 0.2f);

        target.y = p_ripple.transform.position.y;
        target.x *= particleOffset;
        target.z *= particleOffset;
        p_ripple.transform.position = target;
        p_ripple.Play();
        fishTrigger.enabled = true;
        
        fishReleaser.Abort();
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();
        fish.TransitionMovement(fishSpeedRange.x, fishRotateSpeedRange.x, 1f);
        fish.ClampTargetPos();
        fishTrigger.enabled = false;
        fishReleaser.Excute(CommonCoroutine.DelayAction(()=>fish.FollowTransform(true), 3f));
        p_ripple.Stop();
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(fishRect.xMin, 2.2f, fishRect.yMin);
        Vector3 size= new Vector3(fishRect.size.x, 0, fishRect.size.y);
        Gizmos.DrawCube(center, size);
    }
}
