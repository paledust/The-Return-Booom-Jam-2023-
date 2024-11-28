using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledLight : MonoBehaviour
{
    [SerializeField] private Light m_light;
    [SerializeField] private float maxIntensity = 2;
    [SerializeField] private float peakFactor = 2;
    [SerializeField] private float tailFactor = 2;
    [SerializeField] private float life = 2;
    private float lightTimer = 0;
    void Update(){
        lightTimer += Time.deltaTime;
        m_light.intensity = Mathf.Lerp(0, maxIntensity, EasingFunc.Easing.pcurve(lightTimer/life, peakFactor, tailFactor));
        if(lightTimer >= life){
            this.enabled = false;
            m_light.enabled = false;
            m_light.intensity = 0;
            gameObject.SetActive(false);
            LightPools.Call_OnThisRecycle(this);
        }
    }
    public void LightUp(){
        this.enabled = true;
        m_light.enabled = true;
        lightTimer = 0;
    }
}
