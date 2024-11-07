using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField] private Transform startTrans;
    [SerializeField] private Transform endTrans;
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private Vector2 spawnCycle;
    [SerializeField] private float cycleOffset = 2;
    [SerializeField] private int maxClouds = 15;

    private float spawnTimer;
    private float nextCycle;
    private int neededCloud = 0;
    private List<MovingCloud> p_clouds;

    void Start(){
        p_clouds = new List<MovingCloud>();
        nextCycle = spawnCycle.GetRndValueInVector2Range();
    }
    void OnEnable(){
        neededCloud = 0;
        spawnTimer = cycleOffset;

        EventHandler.E_OnCloudOutOfBoundry += CloudOutOfBoundryHandler;
    }
    void OnDisable(){
        EventHandler.E_OnCloudOutOfBoundry -= CloudOutOfBoundryHandler;
    }

    void Update(){
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= nextCycle){
            var pendingCloud = p_clouds.Find(x=>!x.gameObject.activeSelf);
            if(pendingCloud != null)
                PrepareMovingCloud(pendingCloud);
            else{
                if(p_clouds.Count < maxClouds)
                    p_clouds.Add(SpawnCloud());
                else
                    neededCloud ++;
            }

            spawnTimer = 0;
            nextCycle = spawnCycle.GetRndValueInVector2Range();
        }
    }
    MovingCloud SpawnCloud(){
        var cloud = Instantiate(cloudPrefab, transform).GetComponent<MovingCloud>();
        PrepareMovingCloud(cloud);

        return cloud;
    }
    void CleanUp(){
        foreach(var cloud in p_clouds){
            Destroy(cloud.gameObject, 5f);
        }
        p_clouds.Clear();
    }
    void PrepareMovingCloud(MovingCloud cloud){
        cloud.transform.position = startTrans.position+Vector3.forward*Random.Range(-3,3);
        cloud.transform.rotation = Quaternion.Euler(0,Random.Range(0f, 360f), 0);
        cloud.transform.localScale = Vector3.one*Random.Range(0.8f,1.2f);
        var shapeModule = cloud.P_Cloud.shape;
        shapeModule.scale = new Vector3(Random.Range(1f, 3f), 1f, 1f);

        cloud.InitMovement(Vector3.left + Vector3.forward*Random.Range(-0.05f,0.05f), Mathf.Abs(startTrans.position.x - endTrans.position.x));
        cloud.gameObject.SetActive(true);
    }
    void CloudOutOfBoundryHandler(MovingCloud cloud){
        if(neededCloud > 0){
            PrepareMovingCloud(cloud);
            neededCloud --;
        }
    }
}
