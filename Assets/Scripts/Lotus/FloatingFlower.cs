using System.Collections;
using UnityEngine;

public class FloatingFlower : MonoBehaviour
{
    [SerializeField] private FloatIdle floatidle;
    [SerializeField] private Transform lotusTrans;
    [SerializeField] private Vector2 lotusScaleRange;
    [SerializeField] private ParticleSystem p_plate;
    [SerializeField] private Animation floatAnimation;
    [SerializeField] private Collider fishDetectTrigger;
    [SerializeField] private GameObject flowerPusher;
    [SerializeField] private Collider pusherDetectTrigger;
[Header("Leaf Growing")]
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private Vector2 releaseForceRange;
    [SerializeField] private Vector2 spawnRange;
    [SerializeField] private Vector2Int amountRange;

    private GrowingLeaf[] leaves;
    private Vector3 velocity;
    private bool bloomed;
    private float moveDist;
    private float currentDist;

    void OnEnable(){
        p_plate.GetComponent<ParticleSeedMatch>().MatchSeed();
        p_plate.Play(true);

        lotusTrans.localScale = Vector3.one * lotusScaleRange.GetRndValueInVector2Range();
    }
    void Update(){
        currentDist += velocity.magnitude*Time.deltaTime;
        if(currentDist >= moveDist){
            LotusManager.Call_OnThisRecycle(this);
            gameObject.SetActive(false);
        }
    }
    void FixedUpdate(){
        transform.position += velocity * Time.fixedDeltaTime;
    }
    public void DetectKoiFish(FishAI fish){
        if(!bloomed) Bloom();
        else ReleaseFlower(fish.transform.rotation * Vector3.forward*Random.Range(0.5f,0.8f));
    }
    void Bloom(){
        p_plate.Stop(true);
        pusherDetectTrigger.enabled = false;
        flowerPusher.SetActive(true);

        StartCoroutine(coroutineStopFlower(()=>{
            int amount = amountRange.GetRndValueInVector2Range();
            leaves = new GrowingLeaf[amount];
            for(int i=0; i<amount; i++){
                var leaf = Instantiate(leafPrefab, transform);
                Vector3 spawnPos = Random.insideUnitCircle;
                spawnPos.z = spawnPos.y;
                spawnPos.y = 0;
                spawnPos = spawnPos.normalized*spawnRange.GetRndValueInVector2Range();
                leaf.transform.localPosition = spawnPos + Vector3.up*0.02f;
                leaf.transform.localRotation = Quaternion.Euler(90,0,0);
                leaf.transform.parent = null;
                leaf.SetActive(true);
                leaves[i] = leaf.GetComponent<GrowingLeaf>();

                var main = p_plate.main;
                main.gravityModifier = 0.01f;
                main = p_plate.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                main.gravityModifier = 0.05f;
            }
            EventHandler.Call_OnFloatingFlowerBloom(this);
            bloomed = true;
        }));
    }
    void ReleaseFlower(Vector3 releaseVelocity){
        floatidle.enabled = false;
        StartCoroutine(coroutineFlowerFloat(releaseVelocity));
        for(int i=0; i<leaves.Length; i++){
            Vector3 pushDir = leaves[i].transform.position - transform.position + releaseVelocity;
            leaves[i].AddForce(pushDir.normalized * releaseForceRange.GetRndValueInVector2Range());
            leaves[i].PlaySinkAnimation();
            leaves[i].gravityFactor = 1f;
        }
        EventHandler.Call_OnFlowerFlow();
    }
    public void PrepareToReleaseFlower()=>fishDetectTrigger.enabled = true;
    public void InitMovement(Vector3 velocity, float moveDist){
        currentDist = 0;
        this.moveDist = moveDist;
        this.velocity = velocity;
    }
    public void MoveFlower(Vector3 moveVec)=>transform.position += moveVec;
    IEnumerator coroutineStopFlower(System.Action OnStopCallback){
        Vector3 initVel = velocity;
        yield return new WaitForLoop(1f, t=>{
            velocity = Vector3.Lerp(initVel, Vector3.zero, t);
        });
        p_plate.Stop(true);

        floatAnimation.Play();
        yield return new WaitForSeconds(floatAnimation.clip.length*0.75f);

        OnStopCallback?.Invoke();
    }
    IEnumerator coroutineFlowerFloat(Vector3 floatVel){
        Vector3 initVel = velocity;
        float duration = Random.Range(0.1f, 0.5f);
        yield return new WaitForLoop(duration, t=>{
            velocity = Vector3.Lerp(initVel, floatVel, EasingFunc.Easing.QuadEaseOut(t));
        });        
    }
}