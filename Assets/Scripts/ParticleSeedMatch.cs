using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSeedMatch : MonoBehaviour
{
    [SerializeField] private ParticleSystem p_target;
    [SerializeField] private ParticleSystem m_particle;
    public void MatchSeed(){
        m_particle.randomSeed = p_target.randomSeed = (uint)Random.Range(-1000, 1000);
    }
}
