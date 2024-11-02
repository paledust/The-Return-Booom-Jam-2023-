using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishMovement{
[Header("Speed")]
    [SerializeField] private float rotateSpeed;
    public float maxSpeed;
[Header("MoveRange")]
    [SerializeField, Min(0)] public float DeaccelarateRange = 0.5f;
    [SerializeField, Min(0)] public float StopRange = 0.2f;
[Header("Visual")]

    private Transform transform;
    private Vector3 direction;
    private Vector3 velocity;

    public Vector3 m_velocity{get{return velocity;}}
    public Vector3 m_direction{get{return direction;}}

    public void Init(Transform transform){
        this.transform = transform;
        direction = transform.rotation * Vector3.forward;
    }   
    public void MoveUpdate(Vector3 moveTarget){
    //Get difference between position and Target in WORLD SPACE.
        Vector3 diff = moveTarget - transform.position;
    //Ignore Y Axis
        diff.y = 0;
    //Clamp Magnitude into for Maximum Speed.
        diff = Vector3.ClampMagnitude(diff, DeaccelarateRange);
        if(DeaccelarateRange>0) diff/=DeaccelarateRange;
        else diff = diff.normalized;
        if(diff.magnitude<StopRange)
            diff = Vector3.zero;

    //Slerp the direction to the clamped difference.
        if(direction==Vector3.zero&&diff!=Vector3.zero)
            direction = transform.rotation * Vector3.right * 0.001f;

        direction = Vector3.Slerp(direction, diff, Time.fixedDeltaTime * rotateSpeed);
        direction.y = 0;
        if((direction-diff).magnitude<=0.001f) direction = diff;
        velocity = direction * diff.magnitude * maxSpeed;

    //Transfrom Direction from 2D-CORRECTED SPACE to WORLD SPACE to get the acctual velocity
        transform.position += velocity * Time.fixedDeltaTime;
    //The Rotation is done in 2D-CORRECTED SPACE, so the direction is not transformed.
        if(direction!=Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
    public void SetPose(Vector3 pos, Vector2 faceDir){
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(faceDir, Vector2.right));
        direction = transform.rotation * Vector3.right;
    }
}
public class FishAI : MonoBehaviour
{
    [SerializeField] private FishMovement fishMovement;
[Header("Target Transform Follow")]
    [SerializeField] private bool followTransform = false;
    [SerializeField] private Transform targetTransform;

    private Vector3 currentTarget;
    
    void OnEnable(){
        currentTarget = transform.position;
        fishMovement.Init(transform);
    }
    void FixedUpdate(){
        if(followTransform && targetTransform!=null) currentTarget = targetTransform.position;
        fishMovement.MoveUpdate(currentTarget);
    }
    public void SetPose(Vector3 pos, Vector2 faceDir){
        fishMovement.SetPose(pos, faceDir);
        currentTarget = transform.position;
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
#if UNITY_EDITOR
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentTarget, 0.05f);
        DebugExtension.DrawArrow(transform.position, fishMovement.m_velocity);
    }
#endif
}