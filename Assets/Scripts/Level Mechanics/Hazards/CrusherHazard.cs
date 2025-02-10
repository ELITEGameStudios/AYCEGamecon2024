using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherHazard : MonoBehaviour
{
    [SerializeField] private Rigidbody2D physics;
    [SerializeField] private float offsetTime, periodicCrushTime, retractTime, crushingTime, crushPause, timer;
    [SerializeField] private Vector2 normalPos, crushedPos;

    private bool TimerElapsed {get{return timer <= 0;}}

    public enum CrusherState{
        IDLE,
        CRUSHING,
        CRUSHED,
        RETRACTING,
    }

    public CrusherState state;

    void Awake(){
        timer = offsetTime;
        state = CrusherState.IDLE;
    }
    
    void Update()
    {
        switch (state){
            case CrusherState.IDLE:
                if(TimerElapsed){
                    timer = crushingTime;
                    state = CrusherState.CRUSHING;
                }
                break;

            case CrusherState.CRUSHING:

                transform.position = Vector2.Lerp(crushedPos, normalPos, timer/crushingTime);

                if(TimerElapsed){ 
                    state = CrusherState.CRUSHED;
                    timer = crushPause;
                } 

                break;

            case CrusherState.CRUSHED:
                if(TimerElapsed){ 
                    state = CrusherState.RETRACTING;
                    timer = crushPause;
                } 
                break;

            case CrusherState.RETRACTING:

                transform.position = Vector2.Lerp(normalPos, crushedPos, timer/retractTime);

                if(TimerElapsed){ 
                    state = CrusherState.IDLE;
                    timer = periodicCrushTime;
                } 

                break;
        } 

        if(!TimerElapsed){timer -= Time.deltaTime;}
    }
    void OnCollisionEnter2D(Collision2D col){
        // if(col.gameObject == Player.main.gameObject && state == CrusherState.CRUSHING){
        //     Player.main.Die();
        // }
    }
}
