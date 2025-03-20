using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventZone : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPlayerEnterZone;
    [SerializeField] private UnityEvent OnPlayerExitZone;
    [SerializeField] private EventZoneType zoneType;

    public enum EventZoneType{
        Default,
        SetLabFootsteps,
        SetCaveFootsteps
    } 


    void OnTriggerEnter2D(Collider2D collision){
        if(collision == Player.main.MainCol){
            OnPlayerEnter();
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision == Player.main.MainCol){
            OnPlayerExit();
        }
    }

    void OnPlayerEnter(){
        switch (zoneType){
            
            case EventZoneType.Default:
                OnPlayerEnterZone.Invoke();
                break;

            case EventZoneType.SetLabFootsteps:
                PlayerAudioManager.instance.SetLabFootsteps();
                break;

            case EventZoneType.SetCaveFootsteps:
                PlayerAudioManager.instance.SetCaveFootsteps();
                break;
        }
    }
    void OnPlayerExit(){
        switch (zoneType){
            case EventZoneType.Default:
                OnPlayerExitZone.Invoke();
                break;
        }
    }
}
