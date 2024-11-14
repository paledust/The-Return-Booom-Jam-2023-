using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAI : MonoBehaviour
{
[Header("Target Transform Follow")]
    [SerializeField] private bool followTransform = false;
    [SerializeField] private Transform targetTransform;
[Header("Speed")]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField, Min(0)] private float DeaccelarateRange = 0.5f;

    private Vector3 currentTarget;
    private Vector3 direction;
    private Vector3 velocity;
    private CoroutineExcuter speedChanger;
    
    void OnEnable(){
        currentTarget = transform.position;
        direction = transform.rotation * Vector3.forward;
        velocity = Vector3.zero;
    }
    void Start(){
        speedChanger = new CoroutineExcuter(this);
    }
    void FixedUpdate(){
        if(followTransform && targetTransform!=null) currentTarget = targetTransform.position;
    //Get difference between position and Target in WORLD SPACE.
        Vector3 diff = currentTarget - transform.position;
    //Ignore Y Axis
        diff.y = 0;
        diff = Vector3.ClampMagnitude(diff, DeaccelarateRange);
        if(DeaccelarateRange>0) diff/=DeaccelarateRange;
        else diff = diff.normalized;
    //Slerp the direction to the clamped difference.
        if(direction==Vector3.zero&&diff!=Vector3.zero)
            direction = transform.rotation * Vector3.forward * 0.001f;

        direction = Vector3.Slerp(direction, diff, Time.fixedDeltaTime * rotateSpeed);
        direction.y = 0;
        if((direction-diff).magnitude<=0.001f) direction = diff;
        velocity = direction.normalized * Mathf.Lerp(minSpeed, maxSpeed, diff.magnitude) * maxSpeed;

        transform.position += velocity * Time.fixedDeltaTime;
        if(direction!=Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
    public void AssignTarget(Vector3 target, bool auto_SwitchFollowMethod = true){
        target.y = transform.position.y;
        currentTarget = target;
        if(auto_SwitchFollowMethod) FollowTransform(false);
    }
    public void AssignTarget(Transform target, bool auto_SwitchFollowMethod = true){
        targetTransform = target;
        if(auto_SwitchFollowMethod) FollowTransform(true);
    }
    public void FollowTransform(bool isFollowTransform)=>followTransform = isFollowTransform;
    public void TransitionMovement(float targetSpeed, float targetRotateSpeed, float duration){
        speedChanger.Excute(coroutineTransitionMovement(targetSpeed, targetRotateSpeed, duration));
    }
    IEnumerator coroutineTransitionMovement(float targetSpeed, float targetRotateSpeed, float duration){
        float initSpeed = maxSpeed;
        float initRotateSpeed = rotateSpeed;

        yield return new WaitForLoop(duration, (t)=>{
            maxSpeed = Mathf.Lerp(initSpeed, targetSpeed, t);
            rotateSpeed = Mathf.Lerp(initRotateSpeed, targetRotateSpeed, t);
        });
    }
#if UNITY_EDITOR
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(currentTarget, 0.05f);
        DebugExtension.DrawArrow(transform.position, velocity);
    }
#endif
}
