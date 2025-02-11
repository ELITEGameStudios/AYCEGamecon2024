using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransferZone : MonoBehaviour
{
    [SerializeField] private string sceneToLoad, sceneToUnload;

    void OnTriggerEnter2D(Collider2D other) {
        if(other == Player.main.MainCol){
            Debug.Log("Player entered scene change zone");
            if(sceneToLoad != ""){SceneSystem.Instance.AddScene(sceneToLoad);}
            if(sceneToUnload != ""){SceneSystem.Instance.UnloadScene(sceneToUnload);}
        }
    }
}
