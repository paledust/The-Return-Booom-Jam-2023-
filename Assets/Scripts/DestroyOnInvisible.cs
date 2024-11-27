using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DestroyOnInvisible : MonoBehaviour
{
    [SerializeField] private float maxAllowSec = 30f;
    private Renderer m_renderer;
    private float timer;
    void Start(){
        timer = Time.time;
        m_renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if(!m_renderer.isVisible){
            if(Time.time-timer > maxAllowSec) Destroy(gameObject, 1);
        }
        else timer = Time.time;
    }
}
