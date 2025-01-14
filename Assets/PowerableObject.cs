using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerableObject : MonoBehaviour
{
    public static List<PowerableObject> objects;
    public bool active {get; private set;}
    [SerializeField] private bool isSwitch; // Can it be toggled on and off repeatedly
    [SerializeField] private UnityEvent onPoweredOnEvent, onPoweredOffEvent;

    void Awake(){
        if(objects == null) { objects = new();}
        objects.Add(this);
    }

    public void Power(PlayerPulse pulseSource){
        if(isSwitch){ active = !active; }
        else if(!active){
            active = true;
            onPoweredOnEvent.Invoke();
        }
    }
}
