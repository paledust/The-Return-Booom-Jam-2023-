using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KoiFishMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect fishRect;
    [SerializeField] private ParticleSystem fishParticles;
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
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*fishRect.width, -y/(ROLL-1.0f)*fishRect.height)+new Vector2(-0.5f*fishRect.width,0.5f*fishRect.height);
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
        location.y = fishParticles.transform.position.y;

        fishParticles.transform.position = location;
        fishParticles.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        fishParticles.Play(true);

        location.y = rippleParticles.transform.position.y;
        rippleParticles.transform.position = location;
        rippleParticles.Play(true);
    }
}
