using System.Collections;
using UnityEngine;

public class FloatingFlower : MonoBehaviour
{
    [SerializeField] private Transform lotusTrans;
    [SerializeField] private Vector2 lotusScaleRange;
    [SerializeField] private ParticleSystem p_plate;
    [SerializeField] private Animation floatAnimation;
[Header("Leaf Growing")]
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private Vector2 spawnRange;
    [SerializeField] private Vector2Int amountRange;

    private Vector3 velocity;
    private float moveDist;
    private float currentDist;

    void OnEnable(){
        p_plate.GetComponent<ParticleSeedMatch>().MatchSeed();
        p_plate.Play(true);

        lotusTrans.localScale = Vector3.one * lotusScaleRange.GetRndValueInVector2Range();
    }
    public void Bloom(){
        p_plate.Stop(true);
        StartCoroutine(coroutineStopFlower(()=>{
            int amount = amountRange.GetRndValueInVector2Range();
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
            }
            EventHandler.Call_OnFloatingFlowerBloom(this);
        }));
    }
    void Update(){
        transform.position += velocity * Time.deltaTime;
        currentDist += velocity.magnitude*Time.deltaTime;
        if(currentDist >= moveDist){
            LotusManager.Call_OnThisRecycle(this);
            gameObject.SetActive(false);
        }
    }
    public void InitMovement(Vector3 velocity, float moveDist){
        currentDist = 0;
        this.moveDist = moveDist;
        this.velocity = velocity;
    }
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
}