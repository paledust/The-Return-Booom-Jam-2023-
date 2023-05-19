using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridConstructMiniGame : MiniGameBasic
{
    [SerializeField] private ParticleSystem m_projectorParticles;
    [SerializeField] private Projector m_projector;
    protected override void OnKeyPressed(Key keyPressed)
    {
        m_projectorParticles.Play(true);
    }
}
