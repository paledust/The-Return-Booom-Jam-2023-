using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererFish : PerRendererBehavior
{
    [ColorUsage(false, true)]public Color EmissionColor;
    private string EmissionColorID = "_EmissionColor";
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetColor(EmissionColorID, EmissionColor);
    }
}
