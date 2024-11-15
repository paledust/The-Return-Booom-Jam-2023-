using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlBirdMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect birdRect;
    [SerializeField] private Transform birdTarget;
[Header("Bird")]
    [SerializeField] private BirdManager birdManager;
[Header("VFX")]
    [SerializeField] private ParticleSystem p_birdCloud;

    private Vector2[] targetPos;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

    protected override void Initialize()
    {
        base.Initialize();
        targetPos = new Vector2[ROLL*LINE];

        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                targetPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*birdRect.width, -y/(ROLL-1.0f)*birdRect.height)+new Vector2(-0.5f*birdRect.width,0.5f*birdRect.height);
            }
        }
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
        if(p_birdCloud.isPlaying) p_birdCloud.Stop();
    }
}
