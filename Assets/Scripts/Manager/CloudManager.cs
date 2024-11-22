using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : Basic_ObjectPool<MovingCloud>
{
[Header("Cloud Spawn Time")]
    [SerializeField] private Transform startTrans;
    [SerializeField] private Transform endTrans;
    [SerializeField] private Vector2 spawnCycle;
    [SerializeField] private float cycleOffset = 2;

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
    public void SpawnOnPos(Vector2 spawnPos){
        var cloud = GetObjFromPool(x=>!x.gameObject.activeSelf);
        Vector3 realpos = cloud.transform.position;
        realpos.x = spawnPos.x;
        realpos.z = spawnPos.y;
        cloud.transform.position = realpos;
    }
    protected override void PrepareTarget(MovingCloud cloud){
        cloud.transform.position = startTrans.position+Vector3.forward*Random.Range(-3,3);
        cloud.transform.rotation = Quaternion.Euler(0,Random.Range(0f, 360f), 0);
        cloud.transform.localScale = Vector3.one*Random.Range(0.8f,1.2f);
        var shapeModule = cloud.P_Cloud.shape;
        shapeModule.scale = new Vector3(Random.Range(1f, 3f), 1f, 1f);

        cloud.InitMovement(Vector3.left + Vector3.forward*Random.Range(-0.05f,0.05f), Mathf.Abs(startTrans.position.x - endTrans.position.x));
        cloud.gameObject.SetActive(true);
    }
}
