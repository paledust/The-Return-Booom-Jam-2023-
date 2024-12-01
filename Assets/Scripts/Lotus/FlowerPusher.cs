using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FlowerPusher : MonoBehaviour
{
    [SerializeField] private float forceScale;
    [SerializeField] private AnimationCurve forceCurve;

    private SphereCollider m_collider;
    private HashSet<FloatingFlower> pendingFlowers = new HashSet<FloatingFlower>();
    
    void Awake(){
        m_collider = GetComponent<SphereCollider>();
    }
    void OnTriggerEnter(Collider other){
        var flower = other.GetComponent<FloatingFlower>();
        if(flower!=null){
            if(!pendingFlowers.Contains(flower))
                pendingFlowers.Add(flower);
        }
    }
    void OnTriggerExit(Collider other){
        var flower = other.GetComponent<FloatingFlower>();
        if(flower!=null){
            if(pendingFlowers.Contains(flower))
                pendingFlowers.Remove(flower);
        }        
    }
    void FixedUpdate(){
        if(pendingFlowers.Count==0||pendingFlowers==null) return;
        foreach(var flower in pendingFlowers){
            Vector3 diff = flower.transform.position - transform.position;
            diff.y = 0;
            Vector3 dir = (diff.z>0)?Vector3.forward:Vector3.back;

            flower.MoveFlower(dir*forceCurve.Evaluate(1-diff.magnitude/(m_collider.radius*m_collider.transform.lossyScale.x))*forceScale*Time.fixedDeltaTime);
        }
    }
}
