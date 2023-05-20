using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectSeedMiniGame : MiniGameBasic
{
    [SerializeField] private Projector[] projectors;
    private Projector[,] projectorMatrix;
    private const int ROLL = 4;
    private const int LINE = 10;
    protected override void Initialize()
    {
        base.Initialize();
        projectorMatrix = new Projector[LINE, ROLL];

        for(int y=0; y<ROLL; y++){
            for(int x=0; x<LINE; x++){
                projectorMatrix[x, y] = projectors[y*ROLL+x];
            }
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        projectorMatrix = null;
    }
    protected override void OnKeyPressed(Key keyPressed){
        
    }
}
