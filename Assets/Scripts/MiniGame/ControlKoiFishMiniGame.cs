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
[Header("Other")]
    [SerializeField] private int triggerBirdFlowerAmount = 2;
    [SerializeField] private int stopCloudFlowerAmount = 3;
    [SerializeField] private int interactionEndFlowerAmount = 4;
    [SerializeField] private CloudManager cloudManager;
    [SerializeField] private LotusManager lotusManager;

    private int bloomedAmount = 0;
    private bool isBirdOut = false;
    private bool isDone = false;
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
        fishTrigger.enabled = false;

        bloomedAmount = 0;
        EventHandler.E_OnFloatingFlowerBloom += FloatingFlowerBloomHandler;
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        EventHandler.E_OnFloatingFlowerBloom -= FloatingFlowerBloomHandler;
    }
    void FloatingFlowerBloomHandler(FloatingFlower flower){
        bloomedAmount ++;
        if(bloomedAmount>=triggerBirdFlowerAmount && !isBirdOut){
            isBirdOut = true;
            EventHandler.E_OnNextMiniGame();
        }
        if(bloomedAmount>=stopCloudFlowerAmount && cloudManager.enabled){
            cloudManager.enabled = false;
        }
        if(bloomedAmount>=interactionEndFlowerAmount && !isDone){
            isDone = true;
            StartCoroutine(CommonCoroutine.DelayAction(()=>{
                lotusManager.FreeLotus();
                lotusManager.enabled = false;
            }, 1f));
            fishReleaser.Abort();
            FishAutopilot();
            fish.DiveIntoWater(-1);

            EventHandler.Call_OnEndMiniGame(this);
        }
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 target;
        target = guidePos[coordinate.y*LINE + coordinate.x];
        target.z = target.y;
        target.y = fish.transform.position.y;

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
        FishAutopilot();
        fishReleaser.Excute(CommonCoroutine.DelayAction(()=>fish.FollowTransform(true), 3f));
    }
    void FishAutopilot(){
        fishTrigger.enabled = false;
        fish.TransitionMovement(fishSpeedRange.x, fishRotateSpeedRange.x, 1f);
        fish.ClampTargetPos();
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
