using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleAudioSystem;

public class Timeline_Util : MonoBehaviour
{
    public void PlayAmbient(string audio_name){
        AudioManager.Instance.PlayAmbience(audio_name, true, 0.5f);
    }
}