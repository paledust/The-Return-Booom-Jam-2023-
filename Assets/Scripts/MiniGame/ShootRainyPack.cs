using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootRainyPack : MiniGameBasic
{
[Header("Control")]
    [SerializeField] private Vector2 fireExplodeTime;
    [SerializeField] private float fireCoolDown=3.4f;
    [SerializeField] private int maxFire = 5;
[Header("Particles")]
    [SerializeField] private float spawnWidth = 5;
    [SerializeField] private GameObject m_particle;
[Header("Gun Audio")]
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private AudioClip[] sfx_shoots;
    [SerializeField] private AudioClip sfx_reload;
    [SerializeField] private AudioClip sfx_explode;
    private int fireCount = 0;
    private float fireTime = 0;
    protected override void Initialize()
    {
        base.Initialize();
        fireTime = Time.time - fireExplodeTime.y;
    }
    protected override void OnKeyPressed(Key keyPressed){
        base.OnKeyPressed(keyPressed);
        if(Time.time > fireTime + fireCoolDown){
            fireTime = Time.time;
            fireCount++;
            ParticleSystem fireSmoke = Instantiate(m_particle.gameObject).GetComponent<ParticleSystem>();
            fireSmoke.transform.position += Vector3.right * Random.Range(-spawnWidth/2f, spawnWidth/2f);
            fireSmoke.Play();

            StartCoroutine(coroutineFireExplode());
            m_audio.PlayOneShot(sfx_shoots[Random.Range(0, sfx_shoots.Length)]);
            m_audio.PlayOneShot(sfx_reload);

            if(fireCount>=maxFire){
                EventHandler.Call_OnEndMiniGame(this);
                EventHandler.Call_OnNextMiniGame();
            }
        }
    }
    IEnumerator coroutineFireExplode(){
        yield return new WaitForSeconds(Random.Range(fireExplodeTime.x, fireExplodeTime.y));
        m_audio.PlayOneShot(sfx_explode);
    }
}
