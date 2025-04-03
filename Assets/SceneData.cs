using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData currentScene;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){
            currentScene = this;
        }
    }

    void OnTriggerStay2D(Collider2D col){
        if(col == Player.main.MainCol && currentScene != this){
            currentScene = this;
        }
    }
}
