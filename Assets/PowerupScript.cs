using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupScript : MonoBehaviour
{
    [SerializeField] private int targetPowerLevel;
    [SerializeField] private bool inRange;
    [SerializeField] private GameObject signifier;
    [SerializeField] private float pressedTargetTime, pressedTimer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inRange){
            if(InputManager.interact.pressed){
                
                pressedTimer -= Time.deltaTime;
                if(pressedTimer <= 0){ ClaimPower(); }
            
            }
            else{ pressedTimer = pressedTargetTime; }
        }
    }

    void ClaimPower(){
        Player.main.SetUnlock(targetPowerLevel);
        Destroy(this);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject == Player.main.gameObject){
            inRange = true;
            signifier.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject == Player.main.gameObject){
            inRange = false;
            signifier.SetActive(false);
        }
    }
}
