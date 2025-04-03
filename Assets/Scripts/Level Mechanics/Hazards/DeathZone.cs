using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    bool immediate = false;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col){
        if(Player.main.MainCol == col){
            Player.main.Die(immediate);
        }
    }
}
