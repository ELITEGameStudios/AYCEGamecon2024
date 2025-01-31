using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField] private Vector2 crushedPos, initPos, crushedOffset;

    [SerializeField] private float offsetInit, cycleTime, holdTime;
    [SerializeField] private float slamTime, retractTime, timer, cycleTimer;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool active = true;
    [SerializeField] private bool Active {get{return active;}}
    [SerializeField] private CrusherCollider crushStatus;


    void Awake(){
        initPos = transform.position;
        crushedPos = initPos + crushedOffset;
    }

    void Start(){
        StartCoroutine(MainCycleCoroutine());
    }

    void Update(){
        if(!TimerCondition) {timer -= Time.deltaTime;}
        cycleTimer += Time.deltaTime;
        if(crushStatus.IsCrushingPlayer){
            Debug.LogAssertion("ur dead dude.");
        }
    }

    public IEnumerator MainCycleCoroutine(){
        yield return new WaitForSeconds(offsetInit);

        while (true){
            // If you ever want to deactivate a crusher, this will hold execution until the programmer decides to re-activate the crusher
            while (!active) { yield return null; }

            // New cycle setup
            cycleTimer = 0;
            transform.position = initPos;


            // Crushing
            timer = slamTime;
            while (!TimerCondition){
                transform.position = Vector2.Lerp(crushedPos, initPos, timer / slamTime);
                yield return null;
            }
            transform.position = crushedPos;

            // Crush hold
            yield return new WaitForSeconds(holdTime);

            // Retracting
            timer = retractTime;
            while (!TimerCondition){
                transform.position = Vector2.Lerp(initPos, crushedPos, timer / retractTime);
                yield return null;
            }
            transform.position = initPos;

            // Wait for next cycle
            while (cycleTimer < cycleTime){yield return null;}
            cycleTimer = 0;
        } 
    }
}
