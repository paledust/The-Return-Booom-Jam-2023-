using System.Collections;
using UnityEngine;

public class FloatingFlower : MonoBehaviour
{
    [SerializeField] private Transform lotusTrans;
    [SerializeField] private Vector2 lotusScaleRange;
    [SerializeField] private ParticleSystem p_plate;
    [SerializeField] private Animation floatAnimation;

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
            floatAnimation.Play();
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

        OnStopCallback?.Invoke();
    }
}