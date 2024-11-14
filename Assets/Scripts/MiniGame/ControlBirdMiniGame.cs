using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBirdMiniGame : MonoBehaviour
{
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect birdRect;
    [SerializeField] private Transform birdTarget;
[Header("Bird")]
    [SerializeField] private BirdManager birdManager;
[Header("VFX")]
    [SerializeField] private ParticleSystem p_birdCloud;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

}
