using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ProceduralBladeGrassRenderer : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    private struct SourceVertex{public Vector3 position;};
    [System.Serializable]
    public class GrassSettings{
        public int maxSegments = 3;
        public Vector2 grassLay = Vector2.zero;
        public float maxBendAngle = 0;
        public float bladeCurvature = 1;
        public float bladeHeight = 1;
        public float bladeHeightVariance = 0.1f;
        public float bladeWidth = 1;
        public float bladeWidthVariance = 0.1f;
    }
    [System.Serializable]
    public class WindSettings{
        public RenderTexture dynamicTexture = null;
        public Texture2D windTexture = null;
        public float windDynamicFactor = 1;
        public float windTextureScale = 1;
        public float windPeriod = 1;
        public float windScale = 1;
        public float windAmplitude = 0;        
    }
    [SerializeField] private Mesh sourceMesh;
    [SerializeField] private ComputeShader bladeGrassCS;
    [SerializeField] private Material material;

    [SerializeField] private GrassSettings grassSettings = default;
    [SerializeField] private WindSettings windSettings = default;
    private bool initialized;
    private ComputeBuffer sourceVertexBuffer;
    private ComputeBuffer sourceTriBuffer;
    private ComputeBuffer drawBuffer;
    private ComputeBuffer argsBuffer;

    private int idBladeGrassKernel;

    private int dispatchSize;

    private Bounds localBounds;

    private const int SOURCE_VERT_STRIDE = sizeof(float)*3;
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float)*(3+(3+1)*3);
    private const int ARGS_STRIDE = sizeof(int) * 4;

    private int[] argsBufferReset = new int[] {0,1,0,0};
    void OnEnable(){
        if(initialized) this.enabled = false;
        initialized = true;

        Vector3[] positions = sourceMesh.vertices;
        int[] tris = sourceMesh.triangles;

        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for(int i=0; i<vertices.Length; i++){
            vertices[i] = new SourceVertex(){
                position = positions[i],
            };
        }
        int numTriangles = tris.Length/3;
        int maxBladeSegments = Mathf.Max(1, grassSettings.maxSegments);
        int maxBladeTriangles = (maxBladeSegments - 1) * 2 + 1;

        sourceVertexBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceVertexBuffer.SetData(vertices);
        sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceTriBuffer.SetData(tris);
        drawBuffer = new ComputeBuffer(numTriangles*maxBladeTriangles, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0); 

        argsBuffer = new ComputeBuffer(1, ARGS_STRIDE, ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(argsBufferReset);

        idBladeGrassKernel = bladeGrassCS.FindKernel("Main");
        bladeGrassCS.SetInt("_MaxBladeSegments", maxBladeSegments);

        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_SourceVertices", sourceVertexBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_SourceTriangles", sourceTriBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_DrawTriangles", drawBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_IndirectArgsBuffer", argsBuffer);

        bladeGrassCS.SetInt("_NumSourceTriangles", numTriangles);
        bladeGrassCS.SetTexture(idBladeGrassKernel, "_WindNoiseTexture", windSettings.windTexture);
        bladeGrassCS.SetTexture(idBladeGrassKernel, "_DynamicTexture", windSettings.dynamicTexture);

        material.SetBuffer("_DrawTriangles", drawBuffer);

        bladeGrassCS.GetKernelThreadGroupSizes(idBladeGrassKernel, out uint threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt((float)numTriangles/threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(grassSettings.bladeHeight);
    }
    void LateUpdate(){
    #if UNITY_EDITOR
        if(!Application.isPlaying){
            OnDisable();
            OnEnable();
        }
    #endif
        drawBuffer.SetCounterValue(0);
        argsBuffer.SetData(argsBufferReset);

        Bounds bounds = TransformBounds(localBounds);

        bladeGrassCS.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);

        bladeGrassCS.SetFloat("_MaxBladeAngle", grassSettings.maxBendAngle);
        bladeGrassCS.SetFloat("_BladeCurvature", grassSettings.bladeCurvature);
        bladeGrassCS.SetFloat("_BladeHeight", grassSettings.bladeHeight);
        bladeGrassCS.SetFloat("_BladeHeightVarriance", grassSettings.bladeHeightVariance);
        bladeGrassCS.SetFloat("_BladeWidth", grassSettings.bladeWidth);
        bladeGrassCS.SetFloat("_BladeWidthVariance", grassSettings.bladeWidthVariance);

        bladeGrassCS.SetFloat("_WindDynamicFactor", windSettings.windDynamicFactor);
        bladeGrassCS.SetFloat("_WindTimeMult", windSettings.windPeriod);
        bladeGrassCS.SetFloat("_WindTexMult", windSettings.windTextureScale);
        bladeGrassCS.SetFloat("_WindPosMult", windSettings.windScale);
        bladeGrassCS.SetFloat("_WindAmplitude", windSettings.windAmplitude);
        bladeGrassCS.SetVector("_GrassLay", grassSettings.grassLay);
        bladeGrassCS.SetVector("_Time", new Vector4(0, Time.timeSinceLevelLoad, 0, 0));
        
        bladeGrassCS.Dispatch(idBladeGrassKernel, dispatchSize, 1, 1);

        Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer, 
                                        0, null, null, UnityEngine.Rendering.ShadowCastingMode.Off, true, gameObject.layer);
    }
    void OnDisable(){
        if(initialized){
            sourceVertexBuffer.Release();
            sourceTriBuffer.Release();
            drawBuffer.Release();
            argsBuffer.Release();
        }
        initialized = false;
    }
    Bounds TransformBounds(Bounds boundsOS){
        var center = transform.TransformPoint(boundsOS.center);

        var extents = boundsOS.extents;
        var axisX   = transform.TransformVector(extents.x, 0, 0);
        var axisY   = transform.TransformVector(0, extents.y, 0);
        var axisZ   = transform.TransformVector(0, 0, extents.z);

        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds() {center = center, extents = extents};
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireMesh(sourceMesh);
    }
}
