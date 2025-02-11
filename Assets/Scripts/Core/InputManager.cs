using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set;}
    
    // Buttons
    public static KeyCode pushButton {get; private set;} = KeyCode.Q;
    public static KeyCode chargeButton {get; private set;} = KeyCode.E;
    public static KeyCode jumpButton {get; private set;} = KeyCode.Space;
    public static KeyCode interactButton {get; private set;} = KeyCode.F;
    public static KeyCode crouchButton {get; private set;} = KeyCode.S;
    public static KeyCode pauseButton {get; private set;} = KeyCode.Escape;
    
    // Thresholds/deadzones
    public static float jumpInputThreshold {get; private set;} = 0.3f;
    
    // Directional input
    public static float X {get; private set;}
    public static float Y {get; private set;}
    public static float XRaw {get; private set;}
    public static float YRaw {get; private set;}
    public static Vector2 direction {get; private set;}
    public static Vector2 directionNonNormal {get; private set;}
    public static Vector2 directionRaw {get; private set;}

    public struct InputState{
        public bool pressedThisFrame;
        public bool pressed;
        public bool releasedThisFrame;
        public KeyCode key;
    }

    public static InputState jump, charge, push, interact, crouch, pause;

    void Awake(){
        if(Instance == null){ Instance = this;}
        else if(Instance != this){ Destroy(this);}
        BindKeys();
    }

    void Update(){

        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        XRaw = Input.GetAxisRaw("Horizontal");
        YRaw = Input.GetAxisRaw("Vertical");

        directionNonNormal = new(X, Y);
        directionRaw = new(XRaw, YRaw);
        direction = directionNonNormal.normalized;

        // Updates all input states being recorded every frame
        UpdateKeyInput( ref jump );
        UpdateKeyInput( ref charge );
        UpdateKeyInput( ref push );
        UpdateKeyInput( ref interact );
        UpdateKeyInput( ref crouch );
        UpdateKeyInput( ref pause );
        
    }

    static void UpdateKeyInput(ref InputState state){    
        
        // Updates the data of an input state using its assigned key
        state.pressedThisFrame = Input.GetKeyDown(state.key);
        state.pressed = Input.GetKey(state.key);
        state.releasedThisFrame = Input.GetKeyUp(state.key);

    }

    void BindKeys(){
        jump.key = jumpButton;
        charge.key = chargeButton;
        push.key = pushButton;
        interact.key = interactButton;
        crouch.key = crouchButton;
        pause.key = pauseButton;
    }

}
