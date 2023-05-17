using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDebrisGenerator : MonoBehaviour
{
    public float spawnIntersection = 1;
    [SerializeField] private GameObject[] stonePrefabs;
    [SerializeField] private float spawnSize = 1;
    [SerializeField] private int maxStoneCount = 50;
    [SerializeField] private Rect spawnZone;
    private List<ConstantForce> stoneDebris;
    private float stoneTime = 0;
    private bool stopSpawn = false;
    [SerializeField, ShowOnly]private int activeStone = 0;
    void Start(){
        stoneDebris = new List<ConstantForce>();
        for(int i=0;i<maxStoneCount;i++){
            int index = i%stonePrefabs.Length;
            var stone = GameObject.Instantiate(stonePrefabs[index]);
            stoneDebris.Add(stone.GetComponent<ConstantForce>());
            stone.hideFlags = HideFlags.HideInInspector;
            stone.transform.position = transform.position + transform.right * Random.Range(-spawnZone.width/2f, spawnZone.width/2f);
            stone.transform.rotation = Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            stone.SetActive(false);
        }
    }
    void Update(){
        if(Time.time>stoneTime+spawnIntersection && !stopSpawn){
            stoneTime = Time.time;
            var stoneTrans = stoneDebris.Find(x=>!x.gameObject.activeSelf).transform;
        //Active one stones from pool
            stoneTrans.position = transform.position + transform.right * Random.Range(-spawnZone.width/2f, spawnZone.width/2f);
            stoneTrans.rotation = Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            stoneTrans.localScale = Vector3.one * spawnSize * Random.Range(0.5f, 1.5f);
            stoneTrans.gameObject.SetActive(true);
            activeStone ++;
        }
        for(int i=0;i<maxStoneCount;i++){
            if(stoneDebris[i].transform.position.z>transform.position.z+spawnZone.height){
                stoneDebris[i].gameObject.SetActive(false);
                activeStone --;
            } 
        }
    }
    public void StopStoneSpawn() => StartCoroutine(coroutineStopStoneSpawn());
    IEnumerator coroutineStopStoneSpawn(){
        stopSpawn = true;
        while(activeStone>0){
            yield return null;
        }
        
        for(int i=0; i<maxStoneCount; i++){
            Destroy(stoneDebris[i].gameObject);
        }
        stoneDebris.Clear();
        gameObject.SetActive(false);
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(spawnZone.xMin, 0, spawnZone.center.y);
        Vector3 size= new Vector3(spawnZone.size.x, 0, spawnZone.size.y);
        Gizmos.DrawCube(center, size);
    }
}
