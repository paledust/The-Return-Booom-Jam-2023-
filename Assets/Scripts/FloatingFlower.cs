using System.Collections;
using UnityEngine;

public class FloatingFlower : MonoBehaviour
{
    [SerializeField] private ParticleSystem p_plate;
    [SerializeField] private Animation floatAnimation;

    private Vector3 velocity;
    private float moveDist;
    private float currentDist;

    void OnEnable(){
        p_plate.GetComponent<ParticleSeedMatch>().MatchSeed();
        p_plate.Play(true);
    }
    public void CaughtByFish(){
        p_plate.Stop(true);
        StartCoroutine(coroutineAfterCaught());
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
    IEnumerator coroutineAfterCaught(){
        Vector3 initVel = velocity;
        yield return new WaitForLoop(0.5f, t=>{
            velocity = Vector3.Lerp(initVel, Vector3.zero, t);
        });
        p_plate.Stop(true);
    }
}