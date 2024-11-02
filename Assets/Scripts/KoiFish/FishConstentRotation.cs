using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishConstentRotation : MonoBehaviour
{
    [SerializeField] private Transform rotRoot;
    public float RotateFreq;
    public float RotateAngle;

    private float seed;
    private float timer;
    private CoroutineExcuter transitioner;

    void Start(){
        timer = 0;
        seed = Random.value;
        transitioner = new CoroutineExcuter(this);
    }
    void Update(){
        timer += Time.deltaTime * RotateFreq;
        rotRoot.localEulerAngles = Vector3.up * (90+Mathf.Sin(timer) * RotateAngle * (1+0.4f*Mathf.PerlinNoise(seed, 0.1f)));
    }
}