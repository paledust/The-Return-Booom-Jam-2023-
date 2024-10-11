using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingStoneMiniGame : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect fishRect;

    private Vector2[] throwPos;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    protected override void Initialize()
    {
        base.Initialize();

        throwPos = new Vector2[ROLL*LINE];
        
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                throwPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*fishRect.width, -y/(ROLL-1.0f)*fishRect.height)+new Vector2(-0.5f*fishRect.width,0.5f*fishRect.height);
            }
        }
    }
}
