using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPulse : MonoBehaviour
{
    [SerializeField] private float chargeRadius, pushRadius, pushForce;
    [SerializeField] private float chargeTime, chargeTimer, pushTime, pushTimer;
    public float ChargeTime {get {return chargeTime;} }
    public float ChargeTimer {get {return chargeTimer;} }
    public float PushTime {get {return pushTime;} }
    public float PushTimer {get {return pushTimer;} }
    public bool ChargeReady {get{return chargeTimer < 0 && Player.main.powerLevel >= 1;}}
    public bool PushReady {get{return pushTimer < 0 && Player.main.powerLevel >= 0;}}
    public bool u{get{return pushTimer < 0;}}

    void Pulse(){
        chargeTimer = chargeTime;
        PowerableObject[] powerables = FindObjectsByType<PowerableObject>(FindObjectsSortMode.None);
        foreach (PowerableObject powerable in powerables)
        {
            if(Vector2.Distance(powerable.transform.position, transform.position) <= chargeRadius ){
                powerable.Power(this);
            }
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, chargeRadius);
        foreach (Collider2D col in colliders)
        {
            if(col.tag == "Climbable"){
                Vector2 closestPos = col.ClosestPoint((Vector2)transform.position);
                Vector2 directionVector = (closestPos - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(closestPos, col.transform.position);
                Player.main.Movement.Magnetize(col);
                break;
            }   
        }
    }
    void Push(){
        pushTimer = pushTime;

        // Gets all colliders in the radius of the push
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pushRadius);
        foreach (Collider2D col in colliders)
        {
            try{
                if(col.attachedRigidbody == Player.main.Rb){continue;}

                // Applies an explosion force to a dynamic rigidbody relative to the players position
                if(col.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic )
                { 
                    Vector2 closestPos = col.ClosestPoint((Vector2)transform.position);
                    Vector2 directionVector = (closestPos - (Vector2)transform.position).normalized;
                    float distance = Vector2.Distance(closestPos, col.transform.position);

                    col.attachedRigidbody.AddForceAtPosition(
                        // This formula just multiplies the direction of the force by the force coefficient 
                        // Then, this gets scaled by how close it is to the player. if the object is close to player, it gets most force.
                        directionVector * pushForce * (pushRadius - distance) / pushRadius, 
                        transform.position,
                        ForceMode2D.Impulse 
                    );
                }
            }
            catch (System.NullReferenceException) { continue; } // Skips any errors based on if the collider has a rigidbody or not. 
        }
    }

    void Update(){
        // Checks for input and the ready state of using either mechanic

        if(!ChargeReady){ chargeTimer -= Time.deltaTime; }
        else if(InputManager.charge.pressed){ Pulse(); }

        if(!PushReady){pushTimer -= Time.deltaTime;}
        else if(InputManager.push.pressed){ Push(); }
    }
}
