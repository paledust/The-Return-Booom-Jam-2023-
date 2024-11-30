using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishMovement{
[Header("Speed")]
    public float rotateSpeed;
    public float maxSpeed;
[Header("MoveRange")]
    [SerializeField, Min(0)] public float DeaccelarateRange = 0.5f;
    [SerializeField, Min(0)] public float StopRange = 0.2f;
[Header("Visual")]
    [SerializeField] private FishConstentRotation fishRotation;
    [SerializeField] private AnimationCurve rotateCurve;
    [SerializeField] private Vector2 rotateFreqRange;
    [SerializeField] private Vector2 rotateAngleRange;
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
    //Clamp Magnitude into for Maximum Speed.
        diff = Vector3.ClampMagnitude(diff, DeaccelarateRange);
        if(DeaccelarateRange>0) diff/=DeaccelarateRange;
        else diff = diff.normalized;
        if(diff.magnitude<StopRange)
            diff = Vector3.zero;

    //Slerp the direction to the clamped difference.
        if(direction==Vector3.zero&&diff!=Vector3.zero)
            direction = transform.rotation * Vector3.forward * 0.001f;

        direction = Vector3.Slerp(direction, diff, Time.fixedDeltaTime * rotateSpeed);
        if((direction-diff).magnitude<=0.001f) direction = diff;
        velocity = direction.normalized * diff.sqrMagnitude * maxSpeed;

        transform.position += velocity * Time.fixedDeltaTime;
        if(direction!=Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

    //Update Rotation
        float realSpeed = velocity.magnitude;
        fishRotation.RotateAngle = Mathf.Lerp(rotateAngleRange.x, rotateAngleRange.y, rotateCurve.Evaluate(realSpeed/maxSpeed));
        fishRotation.RotateFreq = Mathf.Lerp(rotateFreqRange.x, rotateFreqRange.y, rotateCurve.Evaluate(realSpeed/maxSpeed));
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
    private CoroutineExcuter speedChanger;
    
    void OnEnable(){
        currentTarget = transform.position;
        fishMovement.Init(transform);
    }
    void Start(){
        speedChanger = new CoroutineExcuter(this);
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
    public void ClampTargetPos(){
        Vector3 diff = currentTarget - transform.position;
        currentTarget = transform.position + Vector3.ClampMagnitude(diff, fishMovement.DeaccelarateRange);
    }
    public void DiveIntoWater(float diveDepth){
        StartCoroutine(coroutineSinkTarget(diveDepth, 1.5f));
    }
    IEnumerator coroutineSinkTarget(float diveDepth, float duration){
        Vector3 initTarget = transform.position + fishMovement.m_velocity * duration * 2;
        Vector3 finalTarget = initTarget + Vector3.up * diveDepth;
        Vector3 target = initTarget;
        yield return new WaitForLoop(duration, (t)=>{
            target = Vector3.Lerp(initTarget, finalTarget, EasingFunc.Easing.SmoothInOut(t));
            AssignTarget(target);
        });
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(false);
    }
    IEnumerator coroutineTransitionMovement(float targetSpeed, float targetRotateSpeed, float duration){
        float initSpeed = fishMovement.maxSpeed;
        float initRotateSpeed = fishMovement.rotateSpeed;

        yield return new WaitForLoop(duration, (t)=>{
            fishMovement.maxSpeed = Mathf.Lerp(initSpeed, targetSpeed, t);
            fishMovement.rotateSpeed = Mathf.Lerp(initRotateSpeed, targetRotateSpeed, t);
        });
    }
#if UNITY_EDITOR
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentTarget, 0.05f);
        DebugExtension.DrawArrow(transform.position, fishMovement.m_velocity);
    }
#endif
}