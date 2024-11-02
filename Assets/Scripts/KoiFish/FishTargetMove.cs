using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FishTargetMove : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart m_cart;
    [SerializeField] private float targetSpeed = 0.5f;
    [SerializeField] private float noiseStrength = 0.5f;
    [SerializeField] private float noiseFreq = 1f;

    private float seed;

    void Start(){
        seed = Random.Range(0f, 1f);
    }
    void Update()
    {
        m_cart.m_Speed = targetSpeed + noiseStrength * Mathf.Clamp01(Mathf.PerlinNoise(Time.time*noiseFreq, seed)*2-1);
    }
}
