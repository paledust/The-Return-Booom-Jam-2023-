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
        private bool ambience_crossfading = false;
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
        public void PlayAmbience(string audio_name, float volume){
            current_ambience_name = audio_name;
            if(audio_name == string.Empty) ambience_loop.Stop();

            ambience_loop.clip = audioInfo.GetAMBClipByName(audio_name);
            if(ambience_loop.clip != null)
                ambience_loop.volume = volume;
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
        public void CrossFadeAmbience(string audio_name, float transitionTime)=>CrossFadeAmbience(audio_name, ambience_loop.volume, transitionTime);
        public void CrossFadeAmbience(string audio_name, float targetVolume, float transitionTime){
            if(ambience_crossfading) return;
            StartCoroutine(coroutineCrossFadeAmbience(current_ambience_name, audio_name, targetVolume, transitionTime));
        }
        IEnumerator coroutineCrossFadeAmbience(string from_audio, string to_audio, float targetVolume, float transitionTime){
            ambience_crossfading = true;
            if(from_audio!=string.Empty){
                StartCoroutine(coroutineFadeAudio(ambience_loop, 0, transitionTime));
            }

            AudioSource tempAudio = new GameObject("[_TempAmbience]").AddComponent<AudioSource>();
            tempAudio.volume = 0;
            tempAudio.loop   = true;
            tempAudio.clip   = audioInfo.GetAMBClipByName(to_audio);
            tempAudio.outputAudioMixerGroup = ambience_loop.outputAudioMixerGroup;
            tempAudio.Play();
            yield return coroutineFadeAudio(tempAudio, targetVolume, transitionTime);

            ambience_loop.clip = tempAudio.clip;
            ambience_loop.time = tempAudio.time;
            ambience_loop.volume = tempAudio.volume;
            current_ambience_name = to_audio;
            ambience_loop.Play();
            
            Destroy(tempAudio.gameObject);

            ambience_crossfading = false;
        }
        IEnumerator coroutineFadeAudio(AudioSource source, float targetVolume, float transition){
            float initVolume = source.volume;
            for(float t=0; t<1; t+=Time.unscaledDeltaTime/transition){
                source.volume = Mathf.Lerp(initVolume, targetVolume, t);
                yield return null;
            }
            source.volume = targetVolume;
            yield return null;
        }
    }
}
