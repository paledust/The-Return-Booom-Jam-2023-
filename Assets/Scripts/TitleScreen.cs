using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private Animation titleAnime;
    [SerializeField] private CanvasGroup titleScreenGroup;
    [SerializeField] private CanvasGroup[] infoGroup;
    [SerializeField] private AudioClip sfx_start;
    [SerializeField] private AudioSource m_audio;
[Header("Input")]
    [SerializeField] private InputAction press;
    void Start(){
        StartCoroutine(coroutineTitle());
    }
    void OnEnable(){
        press.performed += StartGame;
    }
    void OnDisable(){
        press.performed -= StartGame;
    }
    IEnumerator coroutineTitle(){
        titleAnime.Play();
        yield return new WaitForSeconds(titleAnime.clip.length);
        press.Enable();
    }
    void StartGame(InputAction.CallbackContext context){
        press.Disable();
        m_audio.PlayOneShot(sfx_start);
        this.enabled = false;
        StartCoroutine(coroutineStartGame());
    }
    IEnumerator coroutineStartGame(){
        titleAnime.Play("TitleScreen_Out");
        SimpleAudioSystem.AudioManager.Instance.PlayAmbience("desert", true, 3, 0.5f);
        yield return new WaitForSeconds(5f);
        GameManager.Instance.SwitchingScene("Title","Main");
    }
}
