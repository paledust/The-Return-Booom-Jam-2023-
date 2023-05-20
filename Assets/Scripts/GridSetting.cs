using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSetting : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] private Color gridColor;
    [SerializeField] private Projector gridProjector;
    [SerializeField] private float projectRadius = -1;
    private Material m_mat;
    private Material original_mat;
    private string FadeRadius_Name = "_FadeRadius";
    void OnEnable(){
        original_mat = gridProjector.material;
        if(m_mat == null) {
            m_mat = new Material(original_mat);
            m_mat.name += "_Instance";
        }
        gridProjector.material = m_mat;
    }
    void OnDisable(){
        gridProjector.material = original_mat;
        DestroyImmediate(m_mat);
    }
    void Update(){
        if(m_mat==null || gridProjector==null) return;
        m_mat.SetFloat(FadeRadius_Name, projectRadius);
        m_mat.color = gridColor;
    }
}
