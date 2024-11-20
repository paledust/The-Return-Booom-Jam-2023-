using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerEyeControl : MonoBehaviour{
[Header("Eye Transform")]
    [SerializeField] private float maxEyeAngle = 90;
    [SerializeField] private float minEyeAngle = -20;
    [SerializeField] private Transform upperEye;
    [SerializeField] private Transform lowerEye;

[Header("Blink")]
    [SerializeField] private float eyeCloseTime = 2;
    [SerializeField] private float eyeBlinkDarkTime = 0.1f;
    [SerializeField] private float eyeReopenTime = 2;

[Header("VFX")]
    [SerializeField] private PostProcessVolume blinkPP;

    private float eyeAngleDelta = 0;
    private CoroutineExcuter eyeBlinker;

    void Start(){
        eyeBlinker = new CoroutineExcuter(this);
    }
    public void BlinkEye(Action transitionAction=null, Action callback=null)=>eyeBlinker.Excute(coroutineBlinkEye(eyeBlinkDarkTime, transitionAction, callback));
    public void BlinkEye(float blinkDarkTime, Action transitionAction=null, Action callback=null)=>eyeBlinker.Excute(coroutineBlinkEye(blinkDarkTime, transitionAction, callback));
    IEnumerator coroutineBlinkEye(float darkTime, Action transitionAction, Action callback){
        float initEyeDelta = eyeAngleDelta;
        yield return new WaitForLoop(eyeCloseTime, (t)=>{
            eyeAngleDelta = Mathf.Lerp(0, 1, EasingFunc.Easing.SmoothInOut(t));
            float eyeAngle = Mathf.Lerp(minEyeAngle, maxEyeAngle, (1-eyeAngleDelta));
            upperEye.transform.localRotation = Quaternion.Euler(-eyeAngle,0,0);
            lowerEye.transform.localRotation = Quaternion.Euler(eyeAngle,0,0);
            blinkPP.weight = EasingFunc.Easing.CircEaseOut(eyeAngleDelta);
        });

        transitionAction?.Invoke();
        yield return new WaitForSeconds(darkTime);

        yield return new WaitForLoop(eyeReopenTime, (t)=>{
            eyeAngleDelta = Mathf.Lerp(1, 0, EasingFunc.Easing.CircEaseOut(t));
            float eyeAngle = Mathf.Lerp(minEyeAngle, maxEyeAngle, (1-eyeAngleDelta));
            upperEye.transform.localRotation = Quaternion.Euler(-eyeAngle,0,0);
            lowerEye.transform.localRotation = Quaternion.Euler(eyeAngle,0,0);
            blinkPP.weight = EasingFunc.Easing.SmoothInOut(eyeAngleDelta);
        });
        callback?.Invoke();
    }
}