using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerupScript : MonoBehaviour
{
    [SerializeField] private int targetPowerLevel;
    [SerializeField] private bool inRange;
    [SerializeField] private GameObject signifier;
    [SerializeField] private float pressedTargetTime, pressedTimer;
    [SerializeField] private UnityEvent onClaimEvent;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;


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
        onClaimEvent.Invoke();
        if(source != null){
            source.clip = clip;
            source.volume = AudioSystem.volume;
            source.Play();
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other == Player.main.MainCol){
            inRange = true;
            signifier.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if(other == Player.main.MainCol){
            inRange = false;
            signifier.SetActive(false);
        }
    }
}
