using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyToWaterMiniGame : MiniGameBasic
{
    [SerializeField] private GameObject skyObject;
[Header("Control")]
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect fishRect;
    [SerializeField] private ParticleSystem fishParticles;
    [SerializeField] private ParticleSystem rippleParticles;
[Header("End")]
    [SerializeField] private MeshRenderer skyRenderer;
    [SerializeField] private float fadeOutDuration = 5;
    private Vector2[] spawnPos;
    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;
    protected override void Initialize()
    {
        base.Initialize();

        spawnPos = new Vector2[ROLL*LINE];
        
        skyObject.SetActive(true);
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*fishRect.width, -y/(ROLL-1.0f)*fishRect.height)+new Vector2(-0.5f*fishRect.width,0.5f*fishRect.height);
            }
        }
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 location;
        location = spawnPos[coordinate.y*LINE + coordinate.x];
        location.z = location.y;
        location.y = fishParticles.transform.position.y;

        fishParticles.transform.position = location;
        fishParticles.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        fishParticles.Play(true);

        location.y = rippleParticles.transform.position.y;
        rippleParticles.transform.position = location;
        rippleParticles.Play(true);
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        StartCoroutine(coroutineFadeOutSky(fadeOutDuration));
    }
    IEnumerator coroutineFadeOutSky(float duration){
        Color initCol = skyRenderer.material.color;
        Color targetCol = initCol;
        targetCol.a = 0;

        yield return new WaitForLoop(duration, (t)=>{
            skyRenderer.material.color = Color.Lerp(initCol, targetCol, EasingFunc.Easing.SmoothInOut(t));
        });
        skyRenderer.gameObject.SetActive(false);
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(fishRect.xMin, 0, fishRect.yMin);
        Vector3 size= new Vector3(fishRect.size.x, 0, fishRect.size.y);
        Gizmos.DrawCube(center, size);
    }
}
