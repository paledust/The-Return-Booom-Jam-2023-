using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    void Update()
    {
        transform.position += Vector3.up*speed*Time.deltaTime;        
    }
}
