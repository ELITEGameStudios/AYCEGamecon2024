using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnPoint : MonoBehaviour
{

    public bool restartsScene; 
    public int sceneTarget; 
    public UnityEvent onRespawn; 

    void Start(){
        RespawnSystem.Instance.AddRespawnPoint(this);
    }

    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collider){
        if(collider == Player.main.MainCol){
            RespawnSystem.Instance.PingPoint(this);
        }
    }
}
