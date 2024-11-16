using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotusManager : Basic_ObjectPool<FloatingFlower>
{
[Header("Flower Spawn Time")]
    [SerializeField] private Transform startTrans;
    [SerializeField] private Transform endTrans;
    [SerializeField] private Vector2 spawnCycle;
    [SerializeField] private float cycleOffset = 2;
    [SerializeField] private float flowerSpeed = 1;

    private int bloomedAmount = 0;
    private float spawnTimer;
    private float nextCycle;

    void Start(){
        nextCycle = spawnCycle.GetRndValueInVector2Range();
    }
    void OnEnable(){
        spawnTimer = cycleOffset;
    }
    
    void Update(){
        spawnTimer += Time.deltaTime;
        
        if(spawnTimer >= nextCycle){
            var cloud = GetObjFromPool(x=>!x.gameObject.activeSelf);

            spawnTimer = 0;
            nextCycle = spawnCycle.GetRndValueInVector2Range();
        }
    }
    protected override void PrepareTarget(FloatingFlower flower)
    {
        flower.transform.position = startTrans.position+Vector3.forward*Random.Range(-0.8f,0.8f);
        flower.transform.rotation = Quaternion.Euler(0,Random.Range(0f, 360f), 0);
        flower.transform.localScale = Vector3.one*Random.Range(0.8f,1.2f);
        flower.InitMovement(Vector3.left*Random.Range(0.9f,1.1f)*flowerSpeed, Mathf.Abs(startTrans.position.x - endTrans.position.x));

        flower.gameObject.SetActive(true);
    }
}