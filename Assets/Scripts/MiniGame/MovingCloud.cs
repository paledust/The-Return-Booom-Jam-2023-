using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    [SerializeField] private ParticleSystem p_cloud;

    public ParticleSystem P_Cloud{get{return p_cloud;}}

    private Vector3 velocity;
    private float moveDist;
    private float currentDist;

    void OnEnable()
    {
        p_cloud.Play(true);
    }
    void Update(){
        transform.position += velocity * Time.deltaTime;
        currentDist += velocity.magnitude*Time.deltaTime;
        if(currentDist>=moveDist){
            CloudManager.Call_OnThisRecycle(this);
            gameObject.SetActive(false);
        }
    }
    public void InitMovement(Vector3 velocity, float moveDist){
        currentDist = 0;
        this.moveDist = moveDist;
        this.velocity = velocity;
    }
}
