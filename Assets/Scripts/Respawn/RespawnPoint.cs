using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{

    public bool restartsScene; 

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
