using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobbleBehavior : MonoBehaviour
{
    private float angularSpeed = 0;
    private Vector3 velocity = Vector3.zero;
    private Vector3 fallingVel = Vector3.zero;
    private bool isSinking = false;
    private const float GRAVITY_ACC = 4.9f;
    void FixedUpdate()
    {
        transform.Translate((velocity+fallingVel) * Time.fixedDeltaTime, Space.World);
        transform.Rotate(Vector3.up, angularSpeed * Time.fixedDeltaTime, Space.World);

        if(transform.position.y < ThrowingStoneMiniGame.WATER_HEIGHT){
            if(Mathf.Abs(angularSpeed) > ThrowingStoneMiniGame.MINIMUM_DRIFTANGULAR_SPEED){
                var pos = transform.position;
                pos.y = ThrowingStoneMiniGame.WATER_HEIGHT;
                transform.position = pos;

                fallingVel *= -ThrowingStoneMiniGame.KICK_UP_FACTOR;
                velocity *= ThrowingStoneMiniGame.WATER_SPEED_FRICTION;
                angularSpeed *= ThrowingStoneMiniGame.WATER_ANGULAR_FRICTION;

                EventHandler.Call_OnStoneTouchWater(transform.position);
            }
            else{
                if(!isSinking) {
                    EventHandler.Call_OnStoneTouchWater(transform.position);
                    isSinking = true;
                }
                fallingVel *= ThrowingStoneMiniGame.WATER_SPEED_FRICTION;
                velocity *= 0.98f;
                angularSpeed *= 0.98f;
            }
        }
        fallingVel += GRAVITY_ACC * Vector3.down * Time.fixedDeltaTime;

        if(ThrowingStoneMiniGame.WATER_HEIGHT-transform.position.y>=2) Destroy(gameObject);
    }
    public void ThrowCobble(Vector3 _velocity, float _angularSpeed){
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        angularSpeed = _angularSpeed;
        velocity = _velocity;
    }
}
