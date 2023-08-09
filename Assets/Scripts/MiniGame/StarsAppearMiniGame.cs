using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarsAppearMiniGame : MiniGameBasic
{
    [System.Serializable]
    public class cloud_group{
        public SpriteRenderer spriteRener;
        public Color targetColor;
        public void LerpColor(float progress)=>spriteRener.color = Color.Lerp(Color.clear, targetColor, progress);
    }
    [SerializeField] private GameObject waterGroup;
[Header("Sky")]
    [SerializeField] private cloud_group[] clouds;
    [SerializeField] private MeshRenderer skyRenderer;
    [SerializeField] private Color skyTargetColor;
[Header("Control")]
    [SerializeField] private int brightStarAmount = 200;
    [SerializeField] private float progressLerpSpeed = 5;
    [SerializeField] private ParticleSystem starParticles;
    [SerializeField] private KeyMatrix_SO keyMatrix;
    [SerializeField] private Rect starRect;
    private Vector2[] spawnPos;
    private float progress;

    private const int ROLL = Service.ROLL;
    private const int LINE = Service.LINE;

    protected override void Initialize()
    {
        base.Initialize();

        waterGroup.SetActive(true);

        spawnPos = new Vector2[ROLL*LINE];
        for(int y=0; y<ROLL; y++){
            for(int x=0;x<LINE;x++){
                spawnPos[y*LINE+x] = new Vector2(x/(LINE-1.0f)*starRect.width, -y/(ROLL-1.0f)*starRect.height)+new Vector2(-0.5f*starRect.width,0.5f*starRect.height);
            }
        }
        for(int i=0; i<clouds.Length; i++){
            clouds[i].spriteRener.color = Color.clear;
        }
        skyRenderer.material.color = Color.black;
        progress = 0;

        this.enabled = true;
    }
    protected override void CleanUp()
    {
        base.CleanUp();

        this.enabled = false;
    }
    void Update(){
        float targetProgress = (starParticles.particleCount+0f)/(brightStarAmount+0.0f);
        targetProgress = Mathf.Clamp01(targetProgress);
        progress = Mathf.Lerp(progress, targetProgress, Time.deltaTime * progressLerpSpeed);

        skyRenderer.material.color = Color.Lerp(Color.black, skyTargetColor, progress);
        for(int i=0; i<clouds.Length; i++){
            clouds[i].LerpColor(progress);
        }
        if(progress >= 0.95f){
            EventHandler.Call_OnEndMiniGame(this);
            EventHandler.Call_OnNextMiniGame();
        }
    }
    void OnGUI(){
        GUILayout.Label(progress.ToString());
    }
    protected override void OnKeyPressed(Key keyPressed){
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix.GetCoordinateFromKey(keyPressed);
        Vector3 location;
        location = spawnPos[coordinate.y*LINE + coordinate.x];
        location.z = location.y;
        location.y = starParticles.transform.position.y;

        starParticles.transform.position = location;
        starParticles.Play(true);
    }
    void OnDrawGizmosSelected(){
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color  = new Color(0,1,0,0.2f);
        Vector3 center= new Vector3(starRect.xMin, 0, starRect.yMin);
        Vector3 size= new Vector3(starRect.size.x, 0, starRect.size.y);
        Gizmos.DrawCube(center, size);
    }
}
