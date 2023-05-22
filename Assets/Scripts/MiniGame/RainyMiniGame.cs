using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RainyMiniGame : MiniGameBasic
{
    [SerializeField] private KeyMatrix_SO keyMatrix_Data;
    [SerializeField] private Rect rainArea;
[Header("Particle Control")]
    [SerializeField] private ParticleSystem m_rainParticles;
    [SerializeField] private int Cycle = 3;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float intervel = 0.01f;
    [SerializeField] private int Count = 1;
[Header("End")]
    [SerializeField] private ParticleSystem m_rain_loop;
    private Vector2[] dropPos;
    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    protected override void Initialize()
    {
        base.Initialize();

        dropPos = new Vector2[ROLL*LINE];
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                dropPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*rainArea.width, -y/(ROLL-1.0f)*rainArea.height)+new Vector2(-0.5f*rainArea.width,0.5f*rainArea.height);
            }
        }
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        dropPos = null;
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix_Data.GetCoordinateFromKey(keyPressed);
        Vector3 location = dropPos[coordinate.x+coordinate.y*LINE];
        location.z = location.y;
        location.y = m_rainParticles.transform.position.y;

        StartCoroutine(CoroutineBurstRainDrops(location));
    }
    IEnumerator CoroutineBurstRainDrops(Vector3 pos){
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for(int i=0; i<Cycle; i++){
            float Size = Random.Range(m_rainParticles.main.startSize.constantMin, m_rainParticles.main.startSize.constantMax);
            Vector2 rnd = Random.insideUnitCircle * radius;
            emitParams.position = pos + new Vector3(rnd.x, 0, rnd.y);
            m_rainParticles.Emit(emitParams, Count);
            yield return new WaitForSeconds(intervel);
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(rainArea.xMin, 0, rainArea.yMin);
        Vector3 size= new Vector3(rainArea.size.x, 0, rainArea.size.y);
        Gizmos.DrawCube(center, size);
    }
}
