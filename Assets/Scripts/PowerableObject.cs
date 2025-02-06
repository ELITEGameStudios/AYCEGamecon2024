using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerableObject : MonoBehaviour
{
    public static List<PowerableObject> objects;
    public bool active {get; protected set;}
    [SerializeField] protected bool isSwitch, unaffectedByPulse; // IsSwitch only dictates if it be toggled on and off repeatedly
    [SerializeField] private UnityEvent onPoweredOnEvent, onPoweredOffEvent;

    void Awake(){
        BaseSetup();
    }
    

    protected void BaseSetup(){
        if(objects == null) { objects = new();}
        objects.Add(this);
    }


    // Used by the player pulse mechanic
    public void Power(){
        if(unaffectedByPulse) {return;}
        
        if(isSwitch && active){ 
            active = false; 
            onPoweredOffEvent.Invoke();
        }
        else if(!active){
            active = true;
            onPoweredOnEvent.Invoke();
        }
    }

    // Can only be used if this object is a switch
    // Used by child classes
    public void Power(bool state){ 
        if(!isSwitch || active == state){ return; }
        active = state; 
        
        if(active) { onPoweredOnEvent.Invoke(); }
        else { onPoweredOffEvent.Invoke(); }
    }
}
