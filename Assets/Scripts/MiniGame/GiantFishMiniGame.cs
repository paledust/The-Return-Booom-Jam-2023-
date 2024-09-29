using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiantFishMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect touchRect;
    [SerializeField] private ParticleSystem rippleParticles;
    private Vector2[] spawnPos;
    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    protected override void Initialize()
    {
        base.Initialize();

        spawnPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*touchRect.width, -y/(ROLL-1.0f)*touchRect.height)+new Vector2(-0.5f*touchRect.width,0.5f*touchRect.height);
            }
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
    }
}