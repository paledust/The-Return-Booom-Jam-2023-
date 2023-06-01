using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishConstentRotation : MonoBehaviour
{
    [SerializeField] private Transform rotRoot;
    [SerializeField] private float RotateFreq;
    [SerializeField] private float RotateAngle;
    private float seed;
    void Start(){
        seed = Random.value;
    }
    void Update(){
        rotRoot.localEulerAngles = Vector3.up * (Mathf.Sin(Time.time * RotateFreq)) * RotateAngle * (1+0.4f*Mathf.PerlinNoise(seed, 0.1f));
    }
}
