using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField, NonSerialized] float lastInitOffset; // in order for the editor tooling to work, we must keep track of the last init offset
    [SerializeField] private float initOffset, crushedOffset;

    [SerializeField] private float offsetInit, cycleTime, holdTime;
    [SerializeField] private float slamTime, retractTime, timer, cycleTimer;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool active = true, crushed = false;
    [SerializeField] private bool Active {get{return active;}}
    [SerializeField] private CrusherCollider crushStatus;
    [SerializeField] private CrusherMover mover;

    private void OnValidate()
    {
        if (initOffset != lastInitOffset) {
            mover.Offset = initOffset;
            lastInitOffset = initOffset;
        } else {
            mover.Offset = crushedOffset;
        }
        mover.RefreshBounds();
    }

    void Start(){
        StartCoroutine(MainCycleCoroutine());
    }

    void Update(){
        if(!TimerCondition) {timer -= Time.deltaTime;}
        cycleTimer += Time.deltaTime;
        if(crushStatus.IsCrushingPlayer && crushed){
            Player.main.Die();
        }
    }

    public IEnumerator MainCycleCoroutine(){
        yield return new WaitForSeconds(offsetInit);

        while (true){
            // If you ever want to deactivate a crusher, this will hold execution until the programmer decides to re-activate the crusher
            while (!active) { yield return null; }

            // New cycle setup
            cycleTimer = 0;


            // Crushing
            timer = slamTime;
            while (!TimerCondition){
                mover.Offset = Mathf.Lerp(crushedOffset, initOffset, timer / slamTime);
                yield return null;
            }

            // Crush hold
            crushed = true;
            yield return new WaitForSeconds(holdTime);
            crushed = false;

            // Retracting
            timer = retractTime;
            while (!TimerCondition){
                mover.Offset = Mathf.Lerp(initOffset, crushedOffset, timer / retractTime);
                yield return null;
            }

            // Wait for next cycle
            while (cycleTimer < cycleTime){yield return null;}
            cycleTimer = 0;
        } 
    }
}
