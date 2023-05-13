using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField]
        private AudioInfo_SO audioInfo;
        [SerializeField]
        private AudioSource ambience_loop;
        [SerializeField]
        private AudioSource music_loop;
        string current_ambience_name = string.Empty;
        string current_music_name = string.Empty;
        public void PlayMusic(string audio_name){
            current_music_name = audio_name;
            if(audio_name == string.Empty) music_loop.Stop();

            music_loop.clip = audioInfo.GetBGMClipByName(audio_name);
            if(music_loop.clip!=null)
                music_loop.Play();
        }
        public void PlayAmbience(string audio_name){
            current_ambience_name = audio_name;
            if(audio_name == string.Empty) ambience_loop.Stop();

            ambience_loop.clip = audioInfo.GetAMBClipByName(audio_name);
            if(ambience_loop.clip != null)
                ambience_loop.Play();
        }
        public void PlaySoundEffect(AudioSource targetSource, string audio_name, float volumeScale){
            AudioClip clip = audioInfo.GetSFXClipByName(audio_name);
            if(clip!=null){
                targetSource.PlayOneShot(clip, volumeScale);
            }
        }
        public static void SwitchAudioListener(AudioListener from, AudioListener to){
            from.enabled = false;
            to.enabled = true;
        }
        //TO DO: Maybe adding a function to do cross fading between two different clips
    }
}
