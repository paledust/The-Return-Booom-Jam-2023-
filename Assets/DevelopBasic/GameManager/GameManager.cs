using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
//Please make sure "GameManager" is excuted before every custom script
public class GameManager : Singleton<GameManager>
{
    public static Camera mainCamera;
    [SerializeField] private int targetFrameRate = 60;
[Header("Scene Transition")]
    [SerializeField] private CanvasGroup BlackScreenCanvasGroup;
    [SerializeField] private float transitionDuration = 1;
[Header("Init")]
    [SerializeField] private bool loadInitSceneFromGameManager = false;
    [SerializeField] private string InitScene;
[Header("Demo")]
    [SerializeField] private bool isDemo = true;
    [SerializeField] private bool isTesting = true;
    [SerializeField] private Text demoText;
[Header("Debug")]
    [SerializeField] private InputActionMap debugActions;
    private static bool isSwitchingScene = false;
    private static bool isPaused = false;
    protected override void Awake(){
        base.Awake();
        Application.targetFrameRate = targetFrameRate;
        mainCamera = Camera.main;

        if(loadInitSceneFromGameManager){StartCoroutine(SwitchSceneCoroutine(string.Empty, InitScene));}
        
        debugActions["restart"].performed += Debug_RestartLevel;

        if(isTesting) debugActions.Enable();
    }
    protected override void OnDestroy(){
        base.OnDestroy();

        debugActions["restart"].performed -= Debug_RestartLevel;

        if(debugActions.enabled)debugActions.Disable();
    }
    public void SwitchingScene(string from, string to){
        if(!isSwitchingScene){
            StartCoroutine(SwitchSceneCoroutine(from, to));
        }
    }
    public void SwitchingScene(string to){
        if(!isSwitchingScene){
            StartCoroutine(SwitchSceneCoroutine(to));
        }
    }
#region Game Pause
    public void PauseTheGame(){
        if(isPaused) return;
        
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
    }
    public void ResumeTheGame(){
        if(!isPaused) return;

        AudioListener.pause = false;
        Time.timeScale = 1;
        isPaused = false;
    }
#endregion

    public void EndGame(){
        if(isDemo) demoText.gameObject.SetActive(true);
        StartCoroutine(FadeInScreen(2));
    }
    public void RestartLevel(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(RestartLevel(currentLevel));
    }
    public void RestartLevel_DEBUG(){
        string currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(RestartLevel(currentLevel, true));
    }

#region Scene Transition
    IEnumerator RestartLevel(string level, bool isDebug = false){
        if(isDebug) yield return FadeInScreen(3f);
        isSwitchingScene = true;

        //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
        yield return SceneManager.UnloadSceneAsync(level);
        yield return null;
        //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(level));
        //TO DO: do something after the next scene is loaded. e.g: call event of loading
        yield return FadeOutScreen(transitionDuration);

        isSwitchingScene = false;
    }
    /// <summary>
    /// This method is good for load scene in an additive way, having a persistance scene
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    IEnumerator SwitchSceneCoroutine(string from, string to){
        isSwitchingScene = true;

        if(from != string.Empty){
            //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
            yield return FadeInScreen(transitionDuration);
            yield return SceneManager.UnloadSceneAsync(from);
        }
        //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));
        //TO DO: do something after the next scene is loaded. e.g: call event of loading
        yield return FadeOutScreen(transitionDuration);

        isSwitchingScene = false;
    }
    /// <summary>
    /// This method is good for load one scene each time, no persistance scene
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    IEnumerator SwitchSceneCoroutine(string to){
        isSwitchingScene = true;

        //TO DO: do something before the next scene is loaded. e.g: call event of saving 
        yield return SceneManager.LoadSceneAsync(to);
        //TO DO: do something after the next scene is loaded. e.g: call event of loading

        isSwitchingScene = false;
    }
    public IEnumerator FadeInScreen(float fadeDuration){
        for(float t=0; t<1; t+=Time.deltaTime/fadeDuration){
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(0, 1, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        BlackScreenCanvasGroup.alpha = 1;
    }
    public IEnumerator FadeOutScreen(float fadeDuration){
        for(float t=0; t<1; t+=Time.deltaTime/fadeDuration){
            BlackScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunc.Easing.QuadEaseIn(t));
            yield return null;
        }
        BlackScreenCanvasGroup.alpha = 0;        
    }
#endregion
#region DEBUG ACTION
    void Debug_RestartLevel(InputAction.CallbackContext callback){
        if(callback.ReadValueAsButton()){
            Debug.Log("Test Restart Level");
            RestartLevel();
        }
    }
#endregion
}
