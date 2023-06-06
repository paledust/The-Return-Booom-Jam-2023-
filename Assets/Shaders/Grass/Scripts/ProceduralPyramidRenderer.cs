using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ProceduralPyramidRenderer : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    private struct SourceVertex{
        public Vector3 position;
        public Vector2 uv;
    };
    [SerializeField] private Mesh sourceMesh;
    [SerializeField] private ComputeShader pyramidCS;
    [SerializeField] private Material material;
    [SerializeField] private float pyramidHeight = 1;
    private bool initialized;
    private ComputeBuffer sourceVertexBuffer;
    private ComputeBuffer sourceTriBuffer;
    private ComputeBuffer drawBuffer;
    private ComputeBuffer argsBuffer;

    private int idPyramidKernel;
    private int idTriCountToVertCountKernel;

    private int dispatchSize;

    private Bounds localBounds;

    private const int SOURCE_VERT_STRIDE = sizeof(float)*(3+2);
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float)*(3+(3+2)*3);
    private const int ARGS_STRIDE = sizeof(int) * 4;
    void OnEnable(){
        if(initialized) this.enabled = false;
        initialized = true;

        Vector3[] positions = sourceMesh.vertices;
        Vector2[] uvs = sourceMesh.uv;
        int[] tris = sourceMesh.triangles;

        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for(int i=0; i<vertices.Length; i++){
            vertices[i] = new SourceVertex(){
                position = positions[i],
                uv = uvs[i],
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
        argsBuffer.SetData(new int[]{0,1,0,0});

        idPyramidKernel = pyramidCS.FindKernel("Main");
        idTriCountToVertCountKernel = pyramidCS.FindKernel("TriCountToVertCount");

        pyramidCS.SetBuffer(idPyramidKernel, "_SourceVertices", sourceVertexBuffer);
        pyramidCS.SetBuffer(idPyramidKernel, "_SourceTriangles", sourceTriBuffer);
        pyramidCS.SetBuffer(idPyramidKernel, "_DrawTriangles", drawBuffer);
        pyramidCS.SetInt("_NumSourceTriangles", numTriangles);

        pyramidCS.SetBuffer(idTriCountToVertCountKernel, "_IndirectArgsBuffer", argsBuffer);

        material.SetBuffer("_DrawTriangles", drawBuffer);

        pyramidCS.GetKernelThreadGroupSizes(idPyramidKernel, out uint threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt((float)numTriangles/threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(pyramidHeight);
    }
    void LateUpdate(){
        drawBuffer.SetCounterValue(0);

        Bounds bounds = TransformBounds(localBounds);

        pyramidCS.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
        pyramidCS.SetFloat("_PyramidHeight", pyramidHeight);
        pyramidCS.Dispatch(idPyramidKernel, dispatchSize, 1, 1);

        ComputeBuffer.CopyCount(drawBuffer, argsBuffer, 0);

        pyramidCS.Dispatch(idTriCountToVertCountKernel, 1, 1, 1);
        // int[] array = new int[4];
        // argsBuffer.GetData(array);
        // Debug.Log(drawBuffer.count);


        Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer, 
                                        0, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
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
}
