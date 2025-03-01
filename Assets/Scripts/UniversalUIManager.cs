using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject settingsMenu, mainMenu, splashScreen, pauseMenu;
    [SerializeField] private List<GameObject> menuList;
    [SerializeField] private Image splashImage;
    [SerializeField] private bool inSplash, paused;
    [SerializeField] private bool InMenu {
        get {
            return 
                state == MenuState.MAIN ||
                state == MenuState.SETTINGS ||
                state == MenuState.PAUSED 
            ;}
        }

    [SerializeField] private float splashScreenFadeTime, splashScreenHoldTimeMin;
    [SerializeField] private List<Scene> scenes;
    [SerializeField] private Scene activeScene; 
    [SerializeField] private MenuState state, lastState; 

    public enum MenuState{
        MAIN,
        SETTINGS,
        PAUSED,
        SPLASH,
        NONE
    }



    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
        
        DontDestroyOnLoad(gameObject);
        menuList = new(){mainMenu, settingsMenu, pauseMenu, splashScreen};
    }

    void Start(){

        for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }
        activeScene = SceneManager.GetActiveScene();
        activeScene.isSubScene = true;
        StartCoroutine(SplashScreenCoroutine());
        state = MenuState.SPLASH;
    }

    void Update(){
        // if(InputManager){}
    }

    public void OpenSettings(){
        OpenMenuViaState(MenuState.SETTINGS);
    }
    public void CloseSettings(){
        OpenMenuViaState(lastState);
    }
    
    public void OpenMenuViaState(MenuState newState){
        
        
        lastState = state;
        state = newState;

        try{
            for (int i = 0; i < menuList.Count; i++) { menuList[i].SetActive((int)newState == i); }
        }
        catch{
            // must be a none state
            Debug.Log("No longer in a menu");
        }
        Time.timeScale = InMenu ? 0 : 1;
        
    }

    IEnumerator SwitchToMainScene(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        while (!operation.isDone){
            yield return null;
        }
    }

    IEnumerator SplashScreenCoroutine(){
        Debug.Log("Started");
        yield return new WaitForSeconds(1);

        float timer = splashScreenFadeTime;
        while (timer > 0){ // Fade in
            splashImage.color = Color.Lerp(Color.white,  Color.clear, timer/splashScreenFadeTime);

            timer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Finished first phase");
        yield return StartCoroutine(SwitchToMainScene()); // Load main sccene in background
        yield return new WaitForSecondsRealtime(splashScreenHoldTimeMin);
        // Time.timeScale = 0;
        mainMenu.SetActive(true);
        Debug.Log("Loaded scene");
        
        timer = splashScreenFadeTime;
        while (timer > 0){ // Fade out
            splashImage.color = Color.Lerp(Color.clear, Color.white, timer/splashScreenFadeTime);

            timer -= Time.deltaTime;
            yield return null;
        }

        splashImage.color = Color.clear;
        Debug.Log("Finalizing");
        splashScreen.SetActive(false);
        inSplash = false;
        lastState = state;
        state = MenuState.MAIN;
        Debug.Log("Done");
    }
}
