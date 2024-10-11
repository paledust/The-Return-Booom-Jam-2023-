using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobbleBehavior : MonoBehaviour
{
    private float angularSpeed = 0;
    private Vector3 velocity = Vector3.zero;
    
    private const float GRAVITY_ACC = 9.8f;

    void FixedUpdate()
    {
        transform.Translate(velocity * Time.fixedDeltaTime, Space.World);
        transform.Rotate(Vector3.up, angularSpeed * Time.fixedDeltaTime, Space.World);   
    }
    public void ThrowCobble(Vector3 _velocity, float _angularSpeed){
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        angularSpeed = _angularSpeed;
        velocity = _velocity;
    }
}
