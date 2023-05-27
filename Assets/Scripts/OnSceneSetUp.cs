using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleAudioSystem;
public class OnSceneSetUp : MonoBehaviour
{
[Header("Start ambient")]
    [SerializeField] private string startAmbientName;
    [SerializeField] private float transitionTime = 3;
    [SerializeField] private float targetVolume = 0.5f;
    void Start(){
        AudioManager.Instance.CrossFadeAmbience(startAmbientName, targetVolume, transitionTime);
    }
}
