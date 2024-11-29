using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePosMatch : MonoBehaviour
{
    [SerializeField] private ParticleSystem fishParticle;
    [SerializeField] private float posResize;
    private ParticleSystem m_particleSys;
    void Awake(){
        m_particleSys = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        var particles = new ParticleSystem.Particle[fishParticle.main.maxParticles];
        var m_particles = new ParticleSystem.Particle[m_particleSys.main.maxParticles];
        var currentAmount = fishParticle.GetParticles(particles);
        m_particleSys.GetParticles(m_particles);
        

        // // Change only the particles that are alive
        for(int i = 0; i < currentAmount; i++)
        {
            Vector3 pos = particles[i].position;
            pos.x *= posResize;
            pos.z *= posResize;
            m_particles[i].position = pos;
            m_particles[i].rotation = particles[i].rotation;
        }

        m_particleSys.SetParticles(m_particles, currentAmount);
    }
}
