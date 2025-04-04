using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferZone : MonoBehaviour
{
    [SerializeField] private string sceneToLoad, sceneToUnload;
    [SerializeField] private Vector2 offset;
    

    void OnTriggerEnter2D(Collider2D other) {
        if(other == Player.main.MainCol){
            Debug.Log("Player entered scene change zone");
            if(sceneToLoad != "" && !SceneManager.GetSceneByName(sceneToLoad).isLoaded){
                SceneSystem.Instance.AddScene(sceneToLoad, offset);}
                
            if(sceneToUnload != "" && !SceneManager.GetSceneByName(sceneToUnload).isLoaded){
                SceneSystem.Instance.UnloadScene(sceneToUnload, offset);}
        }
    }
}
