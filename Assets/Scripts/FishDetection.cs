using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDetection : MonoBehaviour
{
    [SerializeField] private FloatingFlower floatingFlower;
    void OnTriggerEnter(Collider other){
        if(other.tag == Service.PLAYER_TAG){
            GetComponent<Collider>().enabled = false;
            floatingFlower.Bloom();
        }
    }
}
