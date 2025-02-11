using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    [SerializeField] private List<Scene> scenes;
    [SerializeField] private Scene activeScene; 
    public static SceneSystem Instance { get; private set; }

    void Awake(){
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
        
        DontDestroyOnLoad(gameObject);       
    }

    void UpdateSceneData(){

        activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }

    }

    public void LoadAndUnloadOperation(string loadSceneName, string unloadSceneName){
        StartCoroutine(LoadAdditiveCoroutine(loadSceneName));

        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == unloadSceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i]));
                scenes.RemoveAt(i);
            }
        }
    }

    public void AddScene(string sceneName){
        StartCoroutine(LoadAdditiveCoroutine(sceneName));
    }

    public void UnloadScene(string sceneName){
        
        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == sceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i]));
                scenes.RemoveAt(i);
            }
        }
    }

    IEnumerator LoadAdditiveCoroutine(string sceneName){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone){
            yield return null;
        }
        
        UpdateSceneData();
    }

    IEnumerator UnloadSceneCoroutine(Scene scene){
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        while (!operation.isDone){
            yield return null;
        }

        UpdateSceneData();
    }
}
