using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PaintBlurRenderer), PostProcessEvent.AfterStack, "Custom/PaintBlur")]
public class PaintBlur : PostProcessEffectSettings
{
[Header("Kuwahara Filter")]
    [Range(1,10), Tooltip("Brush Radius.")]
    public IntParameter_NoInterp radius = new IntParameter_NoInterp();
    [Range(0f,100f), Tooltip("Brush Size.")]
    public FloatParameter brushSize = new FloatParameter{value = 0};
    [Range(0f,1f), Tooltip("Darken blurred pixel Value")]
    public FloatParameter darken = new FloatParameter{value=0};
[Space(10), Header("Blur")]
    [Range(0, 2f)]
    public FloatParameter blurAmount = new FloatParameter{value = 0f};
    [Range(15,100)]
    public IntParameter sampleAmount = new IntParameter{value = 15};
    public override bool IsEnabledAndSupported(PostProcessRenderContext context){
        return enabled.value;
    }
}
public sealed class PaintBlurRenderer: PostProcessEffectRenderer<PaintBlur>{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/PaintBlur"));

        sheet.properties.SetInt("_Radius", settings.radius);
        sheet.properties.SetInt("_Sample", settings.sampleAmount);
        sheet.properties.SetFloat("_BlurSize", settings.blurAmount);
        sheet.properties.SetFloat("_BrushSize", settings.brushSize);
        sheet.properties.SetFloat("_Darken", settings.darken);

        var tempTex  = RenderTexture.GetTemporary(context.width, context.height);
        var tempTex2 = RenderTexture.GetTemporary(context.width, context.height);
        context.command.BlitFullscreenTriangle(context.source, tempTex, sheet, 1);
        context.command.BlitFullscreenTriangle(tempTex, tempTex2, sheet, 2);
        context.command.BlitFullscreenTriangle(tempTex2, context.destination, sheet, 0);
        RenderTexture.ReleaseTemporary(tempTex);
        RenderTexture.ReleaseTemporary(tempTex2);
    }
}
