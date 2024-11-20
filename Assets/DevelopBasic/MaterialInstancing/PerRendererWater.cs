using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererWater : PerRendererBehavior
{
    public float normalScale = 0;
    public float darkControl = 0;
    public float fallOffPower = 0.05f;
    private readonly int FallOffPowerID = Shader.PropertyToID("_WaterFalloffPower");
    private readonly int NormalScaleID = Shader.PropertyToID("_NormalScale");
    private readonly int DarkControlID = Shader.PropertyToID("_DarkControl");
    protected override void UpdateProperties()
    {
        base.UpdateProperties();

        mpb.SetFloat(NormalScaleID, normalScale);
        mpb.SetFloat(DarkControlID, darkControl);
        mpb.SetFloat(FallOffPowerID, fallOffPower);
    }
}
