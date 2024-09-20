using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyToWaterMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect rippleRect;
    [SerializeField] private ParticleSystem rippleParticles;
[Header("End")]
    [SerializeField, Range(0, 1)] private float TriggerPercentage = 0.5f;
    [SerializeField] private MeshRenderer skyRenderer;
    [SerializeField] private MeshRenderer waterRenderer;
    [SerializeField] private float fadeOutDuration = 5;
    private Vector2[] spawnPos;
    private bool[] triggerArray;
    private int counter=0;
    private bool disturbed = false;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    private const string NormalScaleName = "_NormalScale";

    protected override void Initialize()
    {
        base.Initialize();

        counter = 0;
        spawnPos = new Vector2[ROLL*LINE];
        triggerArray = new bool[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                triggerArray[y*LINE+x] = false;
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*rippleRect.width, -y/(ROLL-1.0f)*rippleRect.height)+new Vector2(-0.5f*rippleRect.width,0.5f*rippleRect.height);
            }
        }
    }
    protected override void OnAnyKeyPress()
    {
        if(!disturbed){
            disturbed = true;
            StartCoroutine(coroutineFadeWaterFlow(1f));
        }
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 location;
        int index = coordinate.y*LINE + coordinate.x;


        location = spawnPos[index];
        location.z = location.y;
        location.y = rippleParticles.transform.position.y;
        rippleParticles.transform.position = location;
        rippleParticles.Play(true);

        if(!triggerArray[index]){
            triggerArray[index] = true;
            counter ++;
        }

        if(counter >= TriggerPercentage*(LINE*ROLL)){
            EventHandler.OnEndMiniGame(this);
            EventHandler.OnNextMiniGame();
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        StartCoroutine(coroutineFadeOutSky(fadeOutDuration));
    }
    IEnumerator coroutineFadeOutSky(float duration){
        Color initCol = skyRenderer.material.color;
        Color targetCol = initCol;
        targetCol.a = 0;

        yield return new WaitForLoop(duration, (t)=>{
            skyRenderer.material.color = Color.Lerp(initCol, targetCol, EasingFunc.Easing.SmoothInOut(t));
        });
        skyRenderer.gameObject.SetActive(false);
    }
    IEnumerator coroutineFadeWaterFlow(float duration){
        yield return new WaitForLoop(duration, (t)=>
            waterRenderer.material.SetFloat(NormalScaleName, Mathf.Lerp(0, 0.01f, EasingFunc.Easing.SmoothInOut(t)))
        );
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(rippleRect.xMin, 0, rippleRect.yMin);
        Vector3 size= new Vector3(rippleRect.size.x, 0, rippleRect.size.y);
        Gizmos.DrawCube(center, size);
    }
}
