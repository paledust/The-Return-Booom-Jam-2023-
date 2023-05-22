using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rain_RT_Camera : MonoBehaviour
{
    [SerializeField] private Camera m_cam;
    void Start(){
        Shader.SetGlobalMatrix("RainDropMatrix", m_cam.projectionMatrix * m_cam.worldToCameraMatrix);
    }
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalMatrix("RainDropMatrix", m_cam.projectionMatrix * m_cam.worldToCameraMatrix);
    }
}
