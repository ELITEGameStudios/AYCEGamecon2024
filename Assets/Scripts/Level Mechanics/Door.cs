using System.Collections;
using System.Collections.Generic;
using SoundSystems;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector2 closedPosTop, initOffset;
    [SerializeField] private Vector2 openPosTop, targetPosTop, startPosTop;
    [SerializeField] private Vector2 openPosBot, targetPosBot, startPosBot, closedPosBot;

    [SerializeField] private Transform topPiece;
    [SerializeField] private Transform botPiece;

    [SerializeField] private float offsetInit;
    [SerializeField] private float moveTime, timer;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool IsMoving {get{return state == DoorState.OPENING || state == DoorState.CLOSING;}}
    [SerializeField] private DoorState state;
    [SerializeField] private DoorState initDoorState = DoorState.NONE;
    [SerializeField] public DoorState State {get {return state;}}
    [SerializeField] public EnvironmentalSound soundData;
    
    
    // If the door should open or close based on the state of powerables
    [SerializeField] private PowerableObject[] powerables;
    private bool PowerableDependent {get {return powerables != null && powerables.Length > 0;}}
    [SerializeField] private int powerableReqAmount;
    private int PowerablesActiveCount {
        get {
            int result = 0;
            foreach (PowerableObject powerable in powerables){
                if(powerable.active){result++;}
            }
            return result;
        }
    }
    private bool PowerableCondition {get {return PowerablesActiveCount >= powerableReqAmount;}}
    [SerializeField] private bool powerableOpen; 
    

    // [SerializeField] private bool Active {get{return active;}}

    public enum DoorState{
        NONE,
        OPEN,
        CLOSED,
        OPENING,
        CLOSING
    }
    // [SerializeField] private CrusherCollider crushStatus;


    void Awake(){
        if(state == DoorState.OPEN){
            openPosTop = topPiece.position;
            closedPosTop = openPosTop + initOffset;

            openPosBot = botPiece.position;
            closedPosBot = openPosBot - initOffset;
        }
        else{
            closedPosTop = topPiece.position;
            openPosTop = closedPosTop + initOffset;

            closedPosBot = botPiece.position;
            openPosBot = closedPosBot - initOffset;
        }
        if(initDoorState == DoorState.OPEN){ Open(true); }
        else if(initDoorState == DoorState.CLOSED){ Close(true); }
        
    }
    void Update(){

        if(IsMoving){

            if(!TimerCondition){
                topPiece.position = Vector2.Lerp(targetPosTop, startPosTop, timer / moveTime);
                botPiece.position = Vector2.Lerp(targetPosBot, startPosBot, timer / moveTime);
                timer -= Time.deltaTime;
            }
            else{
                topPiece.position = targetPosTop;
                botPiece.position = targetPosBot;
                state = state == DoorState.OPENING ? DoorState.OPEN : DoorState.CLOSED;
            }
        }
        else if(PowerableDependent){
            if(PowerableCondition){
                Debug.Log("Something is happening");
                if(powerableOpen && state == DoorState.CLOSED){Open();}
                else if(!powerableOpen && state == DoorState.OPEN){Close();}
            }
            else{
                Debug.Log("Something is no longer happening");
                if(powerableOpen && state == DoorState.OPEN){Close();}
                else if(!powerableOpen && state == DoorState.CLOSED){Open();}
            }
        }
    }

    public void Open(bool startup = false){
        if((IsMoving || state == DoorState.OPEN) && !startup) {return;}
        timer = moveTime;
        targetPosTop = openPosTop;
        targetPosBot = openPosBot;

        startPosTop = closedPosTop;
        startPosBot = closedPosBot;
        state = DoorState.OPENING;
        if(!startup) EnvironmentalSoundSystem.instance.CreateEnvSound("DoorOpen", soundData, soundOrgin: gameObject);
    }

    public void Close(bool startup = false){
        if((IsMoving || state == DoorState.CLOSED) && !startup) {return;}
        timer = moveTime;
        targetPosTop = closedPosTop;
        targetPosBot = closedPosBot;

        startPosTop = openPosTop;
        startPosBot = openPosBot;
        state = DoorState.CLOSING;
    }

    public void Toggle(){
        if(state == DoorState.OPEN) {Close();}
        else {Open();}
    }
}
