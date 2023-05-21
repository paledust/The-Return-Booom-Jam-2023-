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
    [SerializeField] private Vector2Int targetUnit;
    [SerializeField] private ScanSquareUnit[] scanUnits;
    private List<ScanSquareUnit> scannedUnit;
    private ScanSquareUnit[,] scanUnitMatrix;
    private const int ROLL = 4;
    private const int LINE = 10;
    private Vector2Int pressedCoordinate;
    private bool acceptNewScan = true;
    protected override void Initialize(){
        base.Initialize();
        scannedUnit = new List<ScanSquareUnit>();
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
        StartCoroutine(coroutineTurnOffAllScan());
    }
    protected override void OnKeyPressed(Key keyPressed){
        if(acceptNewScan){
        //Get the coordinate by the key
            Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        //if scanned already then don't do anything
            if(scanUnitMatrix[coordinate.x, coordinate.y].Scanned) return;
        //Press the key
            pressedCoordinate = coordinate;
            scanUnitMatrix[pressedCoordinate.x, pressedCoordinate.y].StartScan();
            acceptNewScan = false;
        }
        else{
            scanUnitMatrix[pressedCoordinate.x, pressedCoordinate.y].AbortScan();
            acceptNewScan = true;       
        }
    }
    protected override void OnKeyReleased(Key keyReleased){
        if(!acceptNewScan){
            scanUnitMatrix[pressedCoordinate.x, pressedCoordinate.y].AbortScan();
            acceptNewScan = true;
        }
    }
    public void RefreshScan(){acceptNewScan = true;}
    public bool GetResult(ScanSquareUnit scanUnit){
        scannedUnit.Add(scanUnit);
        for(int y=0; y<ROLL; y++){
            for(int x=0; x<LINE; x++){
                if(scanUnit == scanUnitMatrix[x,y]){
                    if(x == targetUnit.x && y == targetUnit.y){
                        EventHandler.Call_OnEndMiniGame(this);
                    }
                    return x == targetUnit.x && y == targetUnit.y;
                }
            }
        }
        return false;
    }
    IEnumerator coroutineTurnOffAllScan(){
        yield return new WaitForSeconds(5f);
        for(int i=0; i<scannedUnit.Count; i++){
            scannedUnit[i].TurnOffScan();
            yield return new WaitForSeconds(Random.Range(.4f,.6f));
        }
        scannedUnit.Clear();
    }
}
