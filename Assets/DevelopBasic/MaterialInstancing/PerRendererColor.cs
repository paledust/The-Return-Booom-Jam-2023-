using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererColor : PerRendererBehavior
{
    [SerializeField, ColorUsage(true, true)] private Color color;
    private string EMISSION_ID = "_EmissionColor";
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetColor(EMISSION_ID, color);
    }
}
