using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private float flickFreq = 3;
    [SerializeField] private float flickAmp = 0.1f;
    private Light m_light;
    private float intensity;
    private float seed;
    void Start()
    {
        m_light = GetComponent<Light>();
        seed = Random.value;
        intensity = m_light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        m_light.intensity = intensity + flickAmp*(2*Mathf.PerlinNoise(Time.time * flickFreq, seed)-1);
    }
}
