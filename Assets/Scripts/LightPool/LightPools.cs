using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPools : Basic_ObjectPool<PooledLight>
{
    public void SpawnOnPos(Vector3 spawnPos){
        var light = GetObjFromPool(x=>!x.gameObject.activeSelf);
        if(light!=null){
            light.transform.position = spawnPos;
            light.LightUp();
            light.gameObject.SetActive(true);
        }
    }
}
