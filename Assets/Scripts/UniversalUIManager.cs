using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [SerializeField] private UIMenu settingsMenu, mainMenu, splashScreen, pauseMenu;
    [SerializeField] private List<UIMenu> menuList;
    [SerializeField] private Graphic[] splashImages;
    [SerializeField] private bool inSplash, paused, switchingMenus;
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
        state = MenuState.NONE;
        
        DontDestroyOnLoad(gameObject);
        menuList = new(){mainMenu, settingsMenu, pauseMenu, splashScreen};
        foreach (UIMenu menu in menuList) {menu.Initialize();}
    }

    void Start(){

        for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }
        activeScene = SceneManager.GetActiveScene();
        activeScene.isSubScene = true;
        StartCoroutine(SplashScreenCoroutine());
    }

    void Update(){
        if(InputManager.pause.pressedThisFrame){
            OpenMenuViaState( InMenu ? MenuState.NONE : MenuState.PAUSED);
        }

        if(Time.timeScale != 1.0f || Time.timeScale != 0.0f){
            Time.timeScale = Mathf.Clamp(
                Time.timeScale + Time.unscaledDeltaTime * (InMenu ? -3 : 3),
                0.0f,
                1.0f
            );
        }
    }

    public void OpenSettings(){
        OpenMenuViaState(MenuState.SETTINGS);
    }
    public void CloseSettings(){
        OpenMenuViaState(lastState);
    }
    public void Resume(){
        OpenMenuViaState(MenuState.NONE);
    }
    
    public void OpenMenuViaState(MenuState newState, bool doCoroutine = true, bool crossFade = false, float customFadeOut = -1){
        if(switchingMenus) {return;}
        lastState = state;
        state = newState;
        if(doCoroutine){
            StartCoroutine(SwitchMenuCoroutine(crossFade, customFadeOut));
        }

        // try{
        //     for (int i = 0; i < menuList.Count; i++) { menuList[i].SetActive((int)newState == i); }
        // }
        // catch{
        //     // must be a none state
        //     Debug.Log("No longer in a menu");
        // }
        
    }

    IEnumerator SwitchMenuCoroutine(bool crossFade, float customFadeOut = -1){
        bool hasCustomFadeOut = customFadeOut >= 0;

        float fadeOut = 0;
        float fadeIn = 0;

        switchingMenus = true;

        UIMenu newMenu = null;
        UIMenu oldMenu = null;

        for (int i = 0; i < menuList.Count; i++){
            if(i == (int)state){ 
                newMenu = menuList[i];
                fadeIn = newMenu.fadeTime;
            }
            else if(i == (int)lastState){ 
                oldMenu = menuList[i]; 
                fadeOut = hasCustomFadeOut ? customFadeOut : oldMenu.fadeTime;
            }
        }  


        if(oldMenu != null){
            if(crossFade && newMenu != null){
                
                newMenu.gameObject.SetActive(true);
                
                newMenu.SwitchActive(true);
                oldMenu.SwitchActive(false, fadeOut);
                
                yield return new WaitForSecondsRealtime(fadeOut > fadeIn ? 
                    fadeOut : 
                    fadeIn
                );
                
                oldMenu.gameObject.SetActive(false);
                switchingMenus = false;
                yield break;
            }

            oldMenu.SwitchActive(false, fadeOut);
            yield return new WaitForSecondsRealtime(fadeOut);
            oldMenu.gameObject.SetActive(false);

        }
        if(newMenu != null){
            newMenu.SwitchActive(true);
            newMenu.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(fadeIn);
        }
        switchingMenus = false;
    }

    IEnumerator SwitchToMainScene(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        while (!operation.isDone){
            yield return null;
        }
    }

    IEnumerator SplashScreenCoroutine(){
        Debug.Log("Started");
        yield return new WaitForSecondsRealtime(1);
        
        OpenMenuViaState(MenuState.SPLASH, false);
        yield return StartCoroutine(SwitchMenuCoroutine(false));

        Debug.Log("Finished first phase");

        yield return StartCoroutine(SwitchToMainScene()); // Load main sccene in background
        yield return new WaitForSecondsRealtime(splashScreenHoldTimeMin);
        
        Debug.Log("Loaded scene");
        OpenMenuViaState(MenuState.NONE);
        yield return StartCoroutine(SwitchMenuCoroutine(false, 2));
        yield return new WaitForSecondsRealtime(3);
        OpenMenuViaState(MenuState.MAIN);
        
        Debug.Log("Done");
    }
}
