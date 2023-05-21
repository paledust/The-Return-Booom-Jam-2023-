using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectSeedMiniGame : MiniGameBasic
{
    [SerializeField] private KeyMatrix_SO keyMatrix;
[Space(10)]
[Header("Scan Unit Visual")]
    [ColorUsage(true, true)] public Color BlinkColor;
    [ColorUsage(true, true)] public Color ErrorColor;
    [ColorUsage(true, true)] public Color PassColor;
    [ColorUsage(true, true)] public Color BingoColor;
    [SerializeField] private Material scanMaterial;
[Space(10)]
    [SerializeField] private ScanSquareUnit[] scanUnits;
    [SerializeField] private int FoundCount = 5;
    private ScanSquareUnit[,] scanUnitMatrix;
    private const int ROLL = 4;
    private const int LINE = 10;
    private int pressedIndex = -1;
    private int detectAmount = 0;
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
        if(pressedIndex == -1){
            pressedIndex = Random.Range(0, scanUnitMatrix.Length);
            scanUnits[pressedIndex].StartScan();
        }
        else{
            scanUnits[pressedIndex].AbortScan();
            pressedIndex = -1;       
        }
    }
    protected override void OnKeyReleased(Key keyReleased){
        if(pressedIndex != -1){
            scanUnits[pressedIndex].AbortScan();
            pressedIndex = -1;
        }
    }
    public void CountOneScan(){
        pressedIndex = -1;
        detectAmount ++;
        if(detectAmount>=FoundCount){
            Debug.Log("Bingo!");
        }
    }
    public bool GetResult(){
        return detectAmount>=FoundCount;
    }
}
