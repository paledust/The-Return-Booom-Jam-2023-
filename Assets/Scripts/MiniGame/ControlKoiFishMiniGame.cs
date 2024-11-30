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
[Header("Color")]
    [SerializeField] private PerRendererFish fishRenderer;
    [SerializeField, ColorUsage(false, true)] private Color normalColor;
    [SerializeField, ColorUsage(false, true)] private Color controlColor;
[Header("VFX")]
    [SerializeField] private ParticleSystem p_ripple;
    [SerializeField] private float particleOffset = 0.7f;
[Header("Other")]
    [SerializeField] private PerRendererWater waterRender;
    [SerializeField] private int triggerBirdFlowerAmount = 2;
    [SerializeField] private int stopCloudFlowerAmount = 3;
    [SerializeField] private int flowerReleaseAmount = 4;
    [SerializeField] private CloudManager cloudManager;
    [SerializeField] private LotusManager lotusManager;

    private int flowerBloomAmount = 0;
    private int flowerFlowAmount = 0;
    private bool isBirdOut = false;
    private bool isDone = false;
    private Vector2[] guidePos;
    private CoroutineExcuter fishReleaser;
    private CoroutineExcuter fishColorer;

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
        fishColorer = new CoroutineExcuter(this);
        fishTrigger.enabled = false;
        cloudManager.enabled = true;
        lotusManager.enabled = true;

        flowerBloomAmount = 0;
        EventHandler.E_OnFloatingFlowerBloom += FloatingFlowerBloomHandler;
        EventHandler.E_OnFlowerFlow += FlowerFlowHandler;
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        EventHandler.E_OnFloatingFlowerBloom -= FloatingFlowerBloomHandler;
        EventHandler.E_OnFlowerFlow -= FlowerFlowHandler;

        fishColorer.Excute(coroutineFishColor(normalColor, 1f));
    }
    void FlowerFlowHandler(){
        flowerFlowAmount ++;
        if(flowerFlowAmount>=flowerBloomAmount && !isDone){
            isDone = true;
            StartCoroutine(coroutineWaterToSky(10));
            fishReleaser.Abort();
            FishAutopilot();
            fish.DiveIntoWater(-1);

            EventHandler.Call_OnEndMiniGame(this);
            StartCoroutine(CommonCoroutine.DelayAction(()=>EventHandler.Call_OnNextMiniGame(), 5f));
        }
    }
    void FloatingFlowerBloomHandler(FloatingFlower flower){
        flowerBloomAmount ++;
        if(flowerBloomAmount>=triggerBirdFlowerAmount && !isBirdOut){
            isBirdOut = true;
            EventHandler.Call_OnNextMiniGame();
        }
        if(flowerBloomAmount>=stopCloudFlowerAmount && cloudManager.enabled){
            cloudManager.enabled = false;
        }
        if(flowerBloomAmount>=flowerReleaseAmount && lotusManager.enabled){
            lotusManager.enabled = false;
            StartCoroutine(CommonCoroutine.DelayAction(()=>{
                lotusManager.PrepareToFreeLotus();
            }, 1f));
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
    protected override void OnAnyKeyPress()
    {
        base.OnAnyKeyPress();
        fishColorer.Excute(coroutineFishColor(controlColor, 0.2f));
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();
        FishAutopilot();
        fishReleaser.Excute(CommonCoroutine.DelayAction(()=>fish.FollowTransform(true), 3f));
        fishColorer.Excute(coroutineFishColor(normalColor, 1f));
    }
    void FishAutopilot(){
        fishTrigger.enabled = false;
        fish.TransitionMovement(fishSpeedRange.x, fishRotateSpeedRange.x, 1f);
        fish.ClampTargetPos();
        p_ripple.Stop();
    }
    IEnumerator coroutineFishColor(Color targetColor, float duration){
        Color initColor = fishRenderer.EmissionColor;
        yield return new WaitForLoop(duration, (t)=>{
            fishRenderer.EmissionColor = Color.Lerp(initColor, targetColor, EasingFunc.Easing.SmoothInOut(t));
        });
    }
    IEnumerator coroutineWaterToSky(float duration){
        float initScale = waterRender.normalScale;
        yield return new WaitForLoop(duration, (t)=>{
            waterRender.normalScale = Mathf.Lerp(initScale, 0, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(fishRect.xMin, 2.2f, fishRect.yMin);
        Vector3 size= new Vector3(fishRect.size.x, 0, fishRect.size.y);
        Gizmos.DrawCube(center, size);
    }
}
