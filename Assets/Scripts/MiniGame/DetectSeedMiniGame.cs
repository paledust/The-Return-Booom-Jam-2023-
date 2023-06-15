using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

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
[Header("Audio")]
    [SerializeField] private AudioSource sfx_audio;
    [SerializeField] private AudioClip scanningClip;
    [SerializeField] private AudioClip abortClip;
    [SerializeField] private AudioClip scannedClip;
    [SerializeField] private AudioClip bingoClip;
[Space(10)]
    [SerializeField] private Vector2Int targetUnit;
    [SerializeField] private ScanSquareUnit[] scanUnits;
[Header("Ending")]
    [SerializeField] private PlayableDirector m_director;
    private List<ScanSquareUnit> scannedUnit;
    private ScanSquareUnit[,] scanUnitMatrix;
    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    private ScanSquareUnit processingUnit;
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
        StartCoroutine(coroutineTurnOffAllScan());
    }
    protected override void OnKeyPressed(Key keyPressed){
    //Get the coordinate by the key
        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
    //if scanned already then don't do anything
        if(scanUnitMatrix[coordinate.x, coordinate.y].IsScanned||scanUnitMatrix[coordinate.x, coordinate.y].IsScanning) return;
    //Press the key
        if(processingUnit!=null && processingUnit!=scanUnitMatrix[coordinate.x, coordinate.y]){
            processingUnit.AbortScan();
            processingUnit.StopSFX();
            processingUnit.PlaySFX(abortClip);
        }
        processingUnit = scanUnitMatrix[coordinate.x, coordinate.y];
        processingUnit.StartScan();
        processingUnit.PlaySFX(scanningClip);
    }
    protected override void OnKeyReleased(Key keyReleased){
        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyReleased);
        ScanSquareUnit unit = scanUnitMatrix[coordinate.x, coordinate.y];
        if(unit.IsScanning){
            unit.AbortScan();
            unit.StopSFX();
            unit.PlaySFX(abortClip);
        }
        if(processingUnit==unit) {processingUnit = null;}
    }
    public void RefreshScan()=>processingUnit = null;
    public bool GetResult(ScanSquareUnit scanUnit){
        scannedUnit.Add(scanUnit);
        for(int y=0; y<ROLL; y++){
            for(int x=0; x<LINE; x++){
                if(scanUnit == scanUnitMatrix[x,y]){
                    if(x == targetUnit.x && y == targetUnit.y){
                        scanUnit.PlaySFX(bingoClip);
                        EventHandler.Call_OnEndMiniGame(this);
                    }
                    else{
                        scanUnit.StopSFX();
                        scanUnit.PlaySFX(scannedClip);
                    }
                    return x == targetUnit.x && y == targetUnit.y;
                }
            }
        }
        return false;
    }
    IEnumerator coroutineTurnOffAllScan(){
        scanUnitMatrix = null;
        yield return new WaitForSeconds(2.5f);
        for(int i=0; i<scannedUnit.Count-1; i++){
            scannedUnit[i].TurnOffScan();
            yield return new WaitForSeconds(Random.Range(.3f,.5f));
        }
        yield return new WaitForSeconds(2f);
        scannedUnit[scannedUnit.Count-1].TurnOffScan();
        scannedUnit.Clear();

        m_director.Play();
    }
}
