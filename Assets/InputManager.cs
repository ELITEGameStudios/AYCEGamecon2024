using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set;}
    public static KeyCode pulseButton {get; private set;} = KeyCode.E;
    public static KeyCode jumpButton {get; private set;} = KeyCode.E;
    public static float X {get; private set;}
    public static float Y {get; private set;}

    public struct InputState{
        public bool pressedThisFrame;
        public bool pressed;
        public bool releasedThisFrame;
    }

    public static InputState boost;
    public static InputState pulse;

    void Awake(){
        if(Instance == null){ Instance = this;}
        else if(Instance != this){ Destroy(this);}
    }

    // Update is called once per frame
    void Update(){
        
        UpdateKeyInput(
            ref boost.pressedThisFrame,
            ref boost.pressed,
            ref boost.releasedThisFrame,
            jumpButton
        );

        UpdateKeyInput(
            ref pulse.pressedThisFrame,
            ref pulse.pressed,
            ref pulse.releasedThisFrame,
            pulseButton
        );
    }

    void UpdateKeyInput(ref bool thisFrame, ref bool pressed, ref bool releasedThisFrame, KeyCode key){
        thisFrame = Input.GetKeyDown(key);
        pressed = Input.GetKey(key);
        releasedThisFrame  = Input.GetKeyUp(key);
    }
}
