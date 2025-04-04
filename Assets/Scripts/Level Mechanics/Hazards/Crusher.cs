using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField, NonSerialized] float lastInitOffset; // in order for the editor tooling to work, we must keep track of the last init offset
    [SerializeField] private float OpenOffset, crushedOffset;

    [SerializeField] private bool startsClosed = false;
    [SerializeField] private float startsClosedInitTimer = 1.25f; //dont worry about this if it doesnt start closed

    [SerializeField] private float startOffset, cycleTime, holdTime, volumeCoefficient;
    [SerializeField] private float slamTime, retractTime, timer, cycleTimer;
    [SerializeField] private bool TimerCondition {get{return timer <= 0;}}
    [SerializeField] private bool active = true, crushed = false, approxCrushed = false;
    [SerializeField] private bool Active {get{return active;}}
    [SerializeField] private CrusherCollider crushStatus;
    [SerializeField] private CrusherMover mover;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private void OnValidate()
    {
        if (OpenOffset != lastInitOffset) {
            mover.Offset = OpenOffset;
            lastInitOffset = OpenOffset;
        } else {
            mover.Offset = crushedOffset;
        }
        mover.RefreshBounds();
    }

    void Start(){
        if (startsClosed)
            mover.Offset = crushedOffset;

        if(clip != null){source = GetComponent<AudioSource>();}

        if (active)
            StartCoroutine(MainCycleCoroutine());
    }

    void Update(){
        if (active)
        {
            if (!TimerCondition) { timer -= Time.deltaTime; }
            cycleTimer += Time.deltaTime;
            if (crushStatus.IsCrushingPlayer && approxCrushed)
            {
                Player.main.Die(true);
            }
        }
    }

    public IEnumerator MainCycleCoroutine(){
        yield return new WaitForSeconds(startOffset);

        while (true){
            // If you ever want to deactivate a crusher, this will hold execution until the programmer decides to re-activate the crusher
            while (!active) { yield return null; }

            // New cycle setup
            cycleTimer = 0;

            //it should retract first if it starts at the bottom
            if (startsClosed)
            {
                crushed = true;
                yield return new WaitForSeconds(0.6f);
                crushed = false;

                //retract
                timer = retractTime;
                while (!TimerCondition)
                {
                    mover.Offset = Mathf.Lerp(OpenOffset, crushedOffset, timer / retractTime);
                    yield return null;
                }

                while (cycleTimer < cycleTime - startsClosedInitTimer) { yield return null; }
                cycleTimer = 0;

                startsClosed = false;
            }

            // Crushing
            timer = slamTime;
            while (!TimerCondition){
                mover.Offset = Mathf.Lerp(crushedOffset, OpenOffset, timer / slamTime);
                if(Mathf.Abs(crushedOffset - mover.Offset) <= 1.5f){
                    approxCrushed = true;
                }
                
                yield return null;
            }
            if(clip != null){
                source.clip = clip;
                source.volume = AudioSystem.volume * volumeCoefficient;
                source.Play();
            }
            // Crush hold
            crushed = true;
            yield return new WaitForSeconds(holdTime);
            crushed = false;
            approxCrushed = false;

            // Retracting
            timer = retractTime;
            while (!TimerCondition){
                mover.Offset = Mathf.Lerp(OpenOffset, crushedOffset, timer / retractTime);
                yield return null;
            }

            // Wait for next cycle
            while (cycleTimer < cycleTime){yield return null;}
            cycleTimer = 0;
        } 
    }

    public void ToggleCrusher()
    {
        active = !active;

        if (startsClosed)
        {
            StartCoroutine(MainCycleCoroutine());
        }
    }
}
