using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    [SerializeField] private List<Scene> scenes;
    [SerializeField] private Scene activeScene; 
    public static SceneSystem Instance { get; private set; }
    public static List<Object> objectsNotDestroyedOnLoad;

    void Awake(){
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}
        
        DontDestroyOnLoad(gameObject);       
        // AddDontDestroyOnLoad(gameObject);
    }

    public static void AddDontDestroyOnLoad(Object _object){
        if(objectsNotDestroyedOnLoad == null){objectsNotDestroyedOnLoad = new List<Object>();}
        objectsNotDestroyedOnLoad.Add(_object);
    }

    public static void DestroyTheUnderstroyable(){
        for (int i = objectsNotDestroyedOnLoad.Count-1; i > 0; i--){
            DestroyImmediate(objectsNotDestroyedOnLoad[i]);
            objectsNotDestroyedOnLoad.RemoveAt(i);
        }
    }

    void UpdateSceneData(){

        activeScene = SceneManager.GetActiveScene();
        for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }

    }

    public void LoadAndUnloadOperation(string loadSceneName, string unloadSceneName, Vector2 offsetPos = new Vector2()){
        StartCoroutine(LoadAdditiveCoroutine(loadSceneName));

        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == unloadSceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i], offsetPos));
                scenes.RemoveAt(i);
            }
        }
    }

    public void AddScene(string sceneName, Vector2 offsetPos = new Vector2()){
        StartCoroutine(LoadAdditiveCoroutine(sceneName, offsetPos));
    }

    public void LoadMainMenuScene(){
        AudioSystem.Instance.StopAllAudio();
        DestroyTheUnderstroyable();
        
        for (int i = scenes.Count-1; i > 0; i--){
            scenes.RemoveAt(i);
        }

        SceneManager.LoadScene(0);
    }

    public void UnloadScene(string sceneName, Vector2 offsetPos = new Vector2()){
        SceneManager.MoveGameObjectToScene(Player.main.gameObject, activeScene);
        for (int i = 0; i < scenes.Count; i++)
        {
            if(scenes[i].name == sceneName){
                StartCoroutine(UnloadSceneCoroutine(scenes[i], offsetPos));
                scenes.RemoveAt(i);
            }
        }
    }

    IEnumerator LoadAdditiveCoroutine(string sceneName,  Vector2 offsetPos = new Vector2()){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone){ yield return null; }

        if(offsetPos != Vector2.zero){
            OffsetScene(sceneName, offsetPos);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        UpdateSceneData();
    }
    

    void OffsetScene(string sceneName, Vector2 offsetPos){
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        GameObject[] objects = loadedScene.GetRootGameObjects();
        foreach (GameObject obj in loadedScene.GetRootGameObjects()) {  obj.transform.position += (Vector3)offsetPos;  }
    }

    IEnumerator UnloadSceneCoroutine(Scene scene, Vector2 offsetPos){
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        while (!operation.isDone){
            yield return null;
        }
        // if(offsetPos != Vector2.zero){
        //     OffsetScene(SceneManager.GetActiveScene().name, offsetPos);
        // }

        UpdateSceneData();
    }
}
