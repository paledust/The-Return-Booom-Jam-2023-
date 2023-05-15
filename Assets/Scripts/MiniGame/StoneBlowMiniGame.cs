using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlowMiniGame : MiniGameBasic
{
    [SerializeField] private List<ConstantForce> stoneDebris;
    [SerializeField] private Vector2 torqueRange;
    [SerializeField] private float torqueNoise;
    private int count = 0;
    private int totalCount;
    protected override void Initialize()
    {
        base.Initialize();
        totalCount = stoneDebris.Count;
    }
    protected override void OnKeyPressed(KeyCode keyPressed){
        int index = Random.Range(0, stoneDebris.Count);
        ConstantForce debri = stoneDebris[index];

        stoneDebris.RemoveAt(index);
        debri.torque = Vector3.right * (torqueRange.x + Random.Range(0,torqueNoise));

        count ++;
        if(count == totalCount){
            EventHandler.Call_OnEndMiniGame(this);
        }
    }
}
