using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ProceduralBladeGrassRenderer : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    private struct SourceVertex{public Vector3 position;};
    [SerializeField] private Mesh sourceMesh;
    [SerializeField] private ComputeShader bladeGrassCS;
    [SerializeField] private Material material;
    [SerializeField] private float pyramidHeight = 1;
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

        sourceVertexBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceVertexBuffer.SetData(vertices);
        sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceTriBuffer.SetData(tris);
        drawBuffer = new ComputeBuffer(numTriangles*3, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0); 

        argsBuffer = new ComputeBuffer(1, ARGS_STRIDE, ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(argsBufferReset);

        idBladeGrassKernel = bladeGrassCS.FindKernel("Main");

        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_SourceVertices", sourceVertexBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_SourceTriangles", sourceTriBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_DrawTriangles", drawBuffer);
        bladeGrassCS.SetBuffer(idBladeGrassKernel, "_IndirectArgsBuffer", argsBuffer);

        bladeGrassCS.SetInt("_NumSourceTriangles", numTriangles);

        material.SetBuffer("_DrawTriangles", drawBuffer);

        bladeGrassCS.GetKernelThreadGroupSizes(idBladeGrassKernel, out uint threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt((float)numTriangles/threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(pyramidHeight);
    }
    void LateUpdate(){
    #if UNITY_EDITOR
        if(Application.isPlaying == false){
            OnDisable();
            OnEnable();
        }
    #endif
        drawBuffer.SetCounterValue(0);
        argsBuffer.SetData(argsBufferReset);

        Bounds bounds = TransformBounds(localBounds);

        bladeGrassCS.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        bladeGrassCS.SetFloat("_PyramidHeight", pyramidHeight);
        bladeGrassCS.Dispatch(idBladeGrassKernel, dispatchSize, 1, 1);

        // int[] array = new int[4];
        // argsBuffer.GetData(array);
        // Debug.Log(array[0]);

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
