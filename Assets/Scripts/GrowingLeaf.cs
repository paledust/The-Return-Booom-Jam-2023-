using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingLeaf : MonoBehaviour
{
    [SerializeField] private float speedDamp = 0.05f;
    [SerializeField] private Animation sinkAnimation;
    public float gravityFactor = 0;
    private Vector3 velocity = Vector3.zero;

    private readonly static Vector2 growingRange = new Vector2(0.05f, 0.2f);
    private readonly static Vector2 durationRange = new Vector2(0.6f, 1.2f);
    void Start()
    {
        StartCoroutine(coroutineGrowing(growingRange.GetRndValueInVector2Range(), durationRange.GetRndValueInVector2Range()));
    }
    void FixedUpdate(){
        transform.position += velocity * Time.fixedDeltaTime;

        velocity += 0.47f*Vector3.down*gravityFactor*Time.fixedDeltaTime;
        if(velocity!=Vector3.zero) velocity *= 1f-speedDamp;
        if(velocity.sqrMagnitude<=0.00001f) velocity = Vector3.zero;
    }
    public void AddForce(Vector3 force){
        velocity += force;
    }
    public void PlaySinkAnimation()=>sinkAnimation.Play();
    IEnumerator coroutineGrowing(float targetScale, float duration){
        float initScale = transform.localScale.x;
        yield return new WaitForLoop(duration, (t)=>{
            transform.localScale = Vector3.one * Mathf.LerpUnclamped(initScale, targetScale, EasingFunc.Easing.SmoothInOut(t));
        });
    }
}
