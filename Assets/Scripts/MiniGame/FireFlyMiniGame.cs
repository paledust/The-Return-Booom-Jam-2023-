using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Pool;

public class FireFlyMiniGame : MiniGameBasic
{
[Header("Interaction")]
    [SerializeField] private ParticleSystem firefly_particle;
    [SerializeField] private ParticleSystem grass_particle;
    [SerializeField] private KeyMatrix_SO keyMatrix_SO;
    [SerializeField] private Rect windArea;
    [SerializeField] private LightPools lightPools;
[Header("Audio")]
    [SerializeField] private AudioSource sfxAudio;
    [SerializeField] private string grassClips;
[Header("Camera Render")]
    [SerializeField] private Camera RT_Camera;

    private Vector2[] spawnPos;

    protected override void Initialize()
    {
        base.Initialize();
        this.enabled = true;

        spawnPos = new Vector2[ROLL*LINE];

        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*windArea.width, -y/(ROLL-1.0f)*windArea.height)+new Vector2(-0.5f*windArea.width,0.5f*windArea.height);
            }
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();
        RT_Camera.enabled = false;
        RT_Camera.gameObject.SetActive(false);
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix_SO.GetCoordinateFromKey(keyPressed);
        Vector3 location;
        location = spawnPos[coordinate.y*LINE + coordinate.x];
        location.z = location.y;
        location.y = firefly_particle.transform.position.y;

        firefly_particle.transform.position = location;
        firefly_particle.Play();

        lightPools.SpawnOnPos(location+Vector3.up*1f);

        grass_particle.transform.position = location;
        grass_particle.Play();

        AudioManager.Instance.PlaySoundEffect(sfxAudio, grassClips, 0.1f);
    }
    public void EndFireflyMiniGame(){
        EventHandler.Call_OnEndMiniGame(this);
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(windArea.xMin, 0, windArea.yMin);
        Vector3 size= new Vector3(windArea.size.x, 0, windArea.size.y);
        Gizmos.DrawCube(center, size);
    }
}
