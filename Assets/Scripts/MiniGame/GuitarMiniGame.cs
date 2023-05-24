using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GuitarMiniGame : MiniGameBasic
{
    [SerializeField] private KeyMatrix_SO keyMatrix_SO;
[Header("Audio")]
    [SerializeField] private AudioSource guitarSource;
    [SerializeField, Range(0,1)] private float volumeScale;
    [SerializeField] private AudioClip[] D_dim_clips;
    [SerializeField] private AudioClip[] D_maj_clips;
    [SerializeField] private AudioClip[] Dm_clips;
    [SerializeField] private AudioClip[] D_clips;
[Header("Animation")]
    [SerializeField] private Animator m_characterAnime;
    [SerializeField] private float delayExit = 2f;
    private string IsPlayingBool = "IsPlaying";
    private float coordStep = 0;
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        Vector2Int coordinate = keyMatrix_SO.GetCoordinateFromKey(keyPressed);
        coordinate.x = coordinate.x%7;
        switch(coordinate.y){
            case 0:
                guitarSource.PlayOneShot(D_dim_clips[coordinate.x], volumeScale);
                break;
            case 1:
                guitarSource.PlayOneShot(D_maj_clips[coordinate.x], volumeScale);
                break;
            case 2:
                guitarSource.PlayOneShot(Dm_clips[coordinate.x], volumeScale);
                break;
            case 3:
                guitarSource.PlayOneShot(D_clips[coordinate.x], volumeScale);
                break;
        }
    }
    protected override void OnNoKeyPress()
    {
        base.OnNoKeyPress();

        StartCoroutine(coroutineExitPlaying());
    }
    protected override void OnAnyKeyPress()
    {
        base.OnAnyKeyPress();
        StopAllCoroutines();

        m_characterAnime.SetBool(IsPlayingBool, true);
    }
    IEnumerator coroutineExitPlaying(){
        yield return new WaitForSeconds(delayExit);
        m_characterAnime.SetBool(IsPlayingBool, false);
    }
}
