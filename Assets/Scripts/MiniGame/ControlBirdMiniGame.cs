using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlBirdMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect birdRect;
    [SerializeField] private Transform birdTarget;
[Header("Audio")]
    [SerializeField] private AudioSource sfx_swallow;
    [SerializeField] private string swallowClip;

    private Vector2[] targetPos;

    protected override void Initialize()
    {
        base.Initialize();
        targetPos = new Vector2[ROLL*LINE];

        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                targetPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*birdRect.width, -y/(ROLL-1.0f)*birdRect.height)+new Vector2(-0.5f*birdRect.width,0.5f*birdRect.height);
            }
        }
        AudioManager.Instance.PlaySoundEffectLoop(sfx_swallow, swallowClip, 2f, 0.2f);
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 target;
        target = targetPos[coordinate.y*LINE + coordinate.x];
        target.z = target.y;
        target.y = 0;

        birdTarget.position = target;
    }
}
