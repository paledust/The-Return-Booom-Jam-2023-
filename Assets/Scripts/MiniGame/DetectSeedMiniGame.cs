using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectSeedMiniGame : MiniGameBasic
{
    [ColorUsage(true, true)] public Color BlinkColor;
    [ColorUsage(true, true)] public Color ErrorColor;
    [ColorUsage(true, true)] public Color PassColor;
    [ColorUsage(true, true)] public Color BingoColor;
    [SerializeField] private Material scanMaterial;
    [SerializeField] private ScanSquareUnit[] scanUnits;
    private ScanSquareUnit[,] scanUnitMatrix;
    private const int ROLL = 4;
    private const int LINE = 10;
    protected override void Initialize(){
        base.Initialize();
        scanUnitMatrix = new ScanSquareUnit[LINE, ROLL];

        for(int y=0; y<ROLL; y++){
            for(int x=0; x<LINE; x++){
                scanUnitMatrix[x, y] = scanUnits[y*LINE+x];
                scanUnitMatrix[x, y].Init(scanMaterial, this);
            }
        }
    }
    protected override void CleanUp(){
        base.CleanUp();
        scanUnitMatrix = null;
    }
    protected override void OnKeyPressed(Key keyPressed){
        scanUnits[Random.Range(0, scanUnits.Length)].StartScan();
    }
    public bool GetResult(){return false;}
}
