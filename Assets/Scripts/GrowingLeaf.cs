using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingLeaf : MonoBehaviour
{
    private readonly static Vector2 growingRange = new Vector2(0.1f, 0.4f);
    private readonly static Vector2 durationRange = new Vector2(0.6f, 1.2f);
    void Start()
    {
        StartCoroutine(coroutineGrowing(growingRange.GetRndValueInVector2Range(), durationRange.GetRndValueInVector2Range()));
    }
    IEnumerator coroutineGrowing(float targetScale, float duration){
        float initScale = transform.localScale.x;
        yield return new WaitForLoop(duration, (t)=>{
            transform.localScale = Vector3.one * Mathf.LerpUnclamped(initScale, targetScale, EasingFunc.Easing.SmoothInOut(t));
        });
    }
}
