using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSetting : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] private Color gridColor;
    [SerializeField] private Projector gridProjector;
    [SerializeField] private Material original_mat;
    [SerializeField] private float projectRadius = -1;
    private Material m_mat;
    private string FadeRadius_Name = "_FadeRadius";
    void OnEnable(){ 
        if(m_mat == null) {
            m_mat = Instantiate(original_mat);
        }
        gridProjector.material = m_mat;
    }
    void OnDisable(){
        gridProjector.material = original_mat;
        DestroyImmediate(m_mat);
    }
    void Update(){
        if(m_mat==null || gridProjector==null || original_mat==null) return;
        m_mat.SetFloat(FadeRadius_Name, projectRadius);
        m_mat.color = gridColor;
    }
}
