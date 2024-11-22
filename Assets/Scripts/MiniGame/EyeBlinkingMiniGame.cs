using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EyeBlinkingMiniGame : MiniGameBasic
{
[Header("Cloud")]
    [SerializeField] private CloudManager cloudManager;
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect cloudRect;
[Header("Eye")]
    [SerializeField] private PlayerEyeControl eyeControl;
    [SerializeField] private Vector2Int blinkTimeRange;
    [SerializeField] private Vector2 blinkDurationRange;
    [SerializeField] private float blinkIntersect = 0.7f;
    [SerializeField] private float minBlinkStep;

    private Vector2[] cloudPos;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

    private float blinkTime;

    protected override void Initialize()
    {
        base.Initialize();
        cloudPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                cloudPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*cloudRect.width, -y/(ROLL-1.0f)*cloudRect.height)+new Vector2(-0.5f*cloudRect.width,0.5f*cloudRect.height);
            }
        }
        
        blinkTime = Time.time - minBlinkStep;
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);
        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector2 pos = cloudPos[coordinate.x + coordinate.y*LINE];
        if(Time.time - blinkTime > minBlinkStep){
            blinkTime = Time.time;
            StartCoroutine(coroutineBlink(pos));
        }
    }
    IEnumerator coroutineBlink(Vector2 cloudSpawnPos){
        float blinkDuration = blinkDurationRange.GetRndValueInVector2Range();
        eyeControl.BlinkEye(blinkDuration, ()=>cloudManager.SpawnOnPos(cloudSpawnPos), null);

        int blinkCount = blinkTimeRange.GetRndValueInVector2Range();
        for(int i=1; i<blinkCount; i++){
            yield return new WaitForSeconds(blinkIntersect);
            eyeControl.BlinkEye(blinkDuration, null, null);
        }
    }
}
