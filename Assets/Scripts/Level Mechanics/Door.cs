using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector2 closedPos, initOffset;
    [SerializeField] private Vector2 openPos, targetPos, startPos;

    [SerializeField] private float offsetInit;
    [SerializeField] private float moveTime, timer;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool IsMoving {get{return state == DoorState.OPENING || state == DoorState.CLOSING;}}
    [SerializeField] private DoorState state;
    
    
    // If the door should open or close based on the state of powerables
    [SerializeField] private PowerableObject[] powerables;
    private bool PowerableDependent {get {return powerables != null;}}
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

    enum DoorState{
        OPEN,
        CLOSED,
        OPENING,
        CLOSING
    }
    // [SerializeField] private CrusherCollider crushStatus;


    void Awake(){
        if(state == DoorState.OPEN){
            openPos = transform.position;
            closedPos = openPos + initOffset;
        }
        else{
            closedPos = transform.position;
            openPos = closedPos + initOffset;
        }
    }
    void Update(){

        if(IsMoving){

            if(!TimerCondition){
                transform.position = Vector2.Lerp(targetPos, startPos, timer / moveTime);
                timer -= Time.deltaTime;
            }
            else{
                transform.position = targetPos;
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

    public void Open(){
        if(IsMoving || state == DoorState.OPEN) {return;}
        timer = moveTime;
        targetPos = openPos;
        startPos = closedPos;
        state = DoorState.OPENING;
    }

    public void Close(){
        if(IsMoving || state == DoorState.CLOSED) {return;}
        timer = moveTime;
        targetPos = closedPos;
        startPos = openPos;
        state = DoorState.CLOSING;
    }

    public void Toggle(){
        if(state == DoorState.OPEN) {Close();}
        else {Open();}
    }
}
