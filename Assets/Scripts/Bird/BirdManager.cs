using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [SerializeField] private BirdAI[] birds;
    [SerializeField] private Transform headTarget;
    [SerializeField] private float spreadAngle;
    [SerializeField] private float spreadDist;
    [SerializeField] private float alignmentFactor = 10;
    private Vector3[] birdTargets;

    void Start()
    {
        birdTargets = new Vector3[birds.Length];

        for(int i=0; i<birdTargets.Length; i++){
            if(i==0) birdTargets[i] = headTarget.position;
            else{
                int headIndex = Mathf.Max(0,i-2);
                birdTargets[i] = birdTargets[headIndex] - Quaternion.Euler(0,((i%2==0)?1:-1)*spreadAngle,0)*Vector3.forward*spreadDist;
            }
        }    
    }

    void Update()
    {
        for(int i=0; i<birdTargets.Length; i++){
            if(i==0) birdTargets[i] = headTarget.position;
            else{
                int headIndex = Mathf.Max(0,i-2);
                birdTargets[i] = birds[headIndex].transform.position - Quaternion.Euler(0,((i%2==0)?1:-1)*spreadAngle,0)*birds[headIndex].transform.forward*spreadDist;
            }
            birds[i].UpdateTarget(birdTargets[i]);
            if(i!=0) birds[i].UpdateAlignment(birds[0].transform.forward*alignmentFactor);
        }    
    }
    
    void OnDrawGizmos(){
        if(birdTargets == null) return;
        for(int i=0; i<birdTargets.Length; i++){
            DebugExtension.DrawPoint(birdTargets[i], new Color((i+0.0f)/6.0f,1,0,0.2f), 1f);
        }
    }
}
