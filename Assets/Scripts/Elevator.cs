using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Vector2 secondPos, firstPos;
    [SerializeField] private Vector2 targetPos, initPos;
    [SerializeField] private float travelSpeed, travelDistance;
    [SerializeField] private float offsetTime, timer, idleTime, initTime;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool active = false, travelDirection;
    [SerializeField] private bool powered;
    [SerializeField] private bool Active {get{return active;}}
    [SerializeField] private ElevatorState state; 
    public ElevatorState State {get{return state;}} 

    public enum ElevatorState{
        RESTING,
        MOVING,
        CLOSED
    }

    void Awake(){
        firstPos = transform.position;
        state = ElevatorState.RESTING;
    }

    public void Activate(){
        if(!powered || active){ return; }

        if((Vector2)transform.position == firstPos){travelDirection = true;}
        else if((Vector2)transform.position == secondPos){travelDirection = false;}


        Debug.Log(gameObject.name + " Elevator Activated, " + travelDirection);
        targetPos = travelDirection ? secondPos : firstPos; 
        initPos = transform.position;
        travelDistance = Vector2.Distance(initPos, targetPos);

        active = true;

        StartCoroutine(MainCycleCoroutine());
    }

    public void Deactivate(bool switchDir){
        if((Vector2)transform.position == targetPos){switchDir = true;}
        if(switchDir) travelDirection = !travelDirection;
        active = false;
        Debug.Log(gameObject.name + " Elevator deactivated");
        StopCoroutine(MainCycleCoroutine());
        state = ElevatorState.RESTING;
    }

    void Start(){

    }

    void FixedUpdate(){
        if(!TimerCondition) {timer -= Time.fixedDeltaTime;}

        if(state == ElevatorState.MOVING){
            transform.position = Vector2.Lerp(targetPos, initPos, timer/initTime);
        }
    }

    public IEnumerator MainCycleCoroutine(){
        state = ElevatorState.CLOSED;
        yield return new WaitForSeconds(offsetTime);
        state = ElevatorState.MOVING;

        timer = travelDistance / travelSpeed;
        initTime = timer;

        // Elevating
        while (!TimerCondition){
            yield return null;
        }
        transform.position = targetPos;

        // Pause if we need
        state = ElevatorState.CLOSED;
        yield return new WaitForSeconds(idleTime);
        Deactivate(true);
    }

    public IEnumerator DoorNumerator(bool open = true){
        yield return null;
    }
}
