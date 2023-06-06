using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPyramidRenderer : MonoBehaviour
{
    [SerializeField] private Mesh sourceMesh = default;
    [SerializeField] private ComputeShader pyramidCS = default;
    [SerializeField] private Material material = default;
    [SerializeField] private float pyramidHeight = 1;
    [SerializeField] private float animationFrequency = 1;
}
