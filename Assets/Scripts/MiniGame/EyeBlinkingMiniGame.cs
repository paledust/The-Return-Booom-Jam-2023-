using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

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
[Header("Blur")]
    [SerializeField] private ParticleSystem p_blurCloud;
[Header("Audio")]
    [SerializeField] private AudioSource swallowAudio;
[Header("End")]
    [SerializeField] private int totalCount = 7;
    [SerializeField] private PlayableDirector dreamEndTimeline;

    private HashSet<Key> pressedKey;
    private Vector2[] cloudPos;
    private float blinkTime;
    private float targetVolume;
    private float volumeStep;
    private CoroutineExcuter volumeFader;

    protected override void Initialize()
    {
        base.Initialize();
        cloudPos = new Vector2[ROLL*LINE];
        pressedKey = new HashSet<Key>();
        volumeFader = new CoroutineExcuter(this);
        targetVolume = swallowAudio.volume;
        volumeStep = targetVolume/(totalCount-1f);
        
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
            targetVolume -= volumeStep;
            volumeFader.Excute(coroutineFadeAudio(targetVolume, 3));

            pressedKey.Add(keyPressed);
            if(pressedKey.Count>=totalCount){
                EventHandler.Call_OnEndMiniGame(this);
                dreamEndTimeline.Play();
            }
        }
    }
    IEnumerator coroutineFadeAudio(float volume, float duration){
        float initVolume = swallowAudio.volume;
        yield return new WaitForLoop(duration, (t)=>{
            swallowAudio.volume = Mathf.Lerp(initVolume, volume, EasingFunc.Easing.SmoothInOut(t));
        });
    }
    IEnumerator coroutineBlink(Vector2 cloudSpawnPos){
        float blinkDuration = blinkDurationRange.GetRndValueInVector2Range();
        eyeControl.BlinkEye(blinkDuration, ()=>{
            cloudManager.SpawnOnPos(cloudSpawnPos);
            
            Vector3 blurPos = cloudSpawnPos;
            blurPos.z = blurPos.y;
            blurPos.y = p_blurCloud.transform.position.y;
            blurPos.x *= 2;
            p_blurCloud.transform.position = blurPos;
            p_blurCloud.Emit(Random.Range(3,6));
        }, null);

        int blinkCount = blinkTimeRange.GetRndValueInVector2Range();
        for(int i=1; i<blinkCount; i++){
            yield return new WaitForSeconds(blinkIntersect);
            eyeControl.BlinkEye(blinkDuration, null, null);
        }
    }
}
