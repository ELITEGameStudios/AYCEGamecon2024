using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPulse : MonoBehaviour
{
    [SerializeField] private float chargeRadius, pushRadius, pushForce, magnetizeRadius;
    [SerializeField] private float chargeTime, chargeTimer, pushTime, pushTimer;
    public float ChargeTime {get {return chargeTime;} }
    public float ChargeTimer {get {return chargeTimer;} }
    public float PushTime {get {return pushTime;} }
    public float PushTimer {get {return pushTimer;} }
    public bool ChargeReady {get{return chargeTimer < 0 && Player.main.powerLevel >= 1;}}
    public bool PushReady {get{return pushTimer < 0 && Player.main.powerLevel >= 0;}}
    public bool u{get{return pushTimer < 0;}}

    private PlayerAnimations animation;

    void Pulse(){
        chargeTimer = chargeTime;
        PowerableObject[] powerables = FindObjectsByType<PowerableObject>(FindObjectsSortMode.None);
        foreach (PowerableObject powerable in powerables)
        {
            if(Vector2.Distance(powerable.transform.position, transform.position) <= chargeRadius ){
                powerable.Power(this);
            }
        }
        TryMagnetize();
        animation.ChangeAnimation("Charging");
        PlayerAudioManager.instance.Pulse();
    }

    void TryMagnetize(){

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetizeRadius);
        foreach (Collider2D col in colliders)
        {
            if(col.tag == "Climbable"){
                Vector2 closestPos = col.ClosestPoint((Vector2)transform.position);
                Vector2 directionVector = (closestPos - (Vector2)transform.position).normalized;
                Debug.Log("Direction: " + directionVector.x + " , " + directionVector.y);
                if(
                    (directionVector.x > 0 && Player.main.Movement.flipDirRaw <= 0) 
                    || (directionVector.x <= 0 && Player.main.Movement.flipDirRaw > 0) )
                {continue;} // the wall is not in the direction the player is facing
                Debug.Log("Passed Check 1");

                float distance = Vector2.Distance(closestPos, col.transform.position);
                RaycastHit2D canMag = Physics2D.Raycast(Player.main.transform.position, directionVector, distance, LayerMask.NameToLayer("Environment"));
                if(canMag != false){    
                    if(canMag.collider.gameObject.name.Contains("PlayableLayer")){
                        continue; // The climbable wall goes phases through the level and is invalid
                    }
                }

                Player.main.Movement.Magnetize(col);
                break;
            }   
        }

    }

    void Push(){
        pushTimer = pushTime;

        // Gets all colliders in the radius of the push
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pushRadius);
        bool hasBoxes = false;
        
        foreach (Collider2D col in colliders){
            if(col.gameObject.tag == "Breakable"){col.GetComponent<Breakable>().BreakBox();}
            hasBoxes = true;
        }

        if(hasBoxes) colliders = Physics2D.OverlapCircleAll(transform.position, pushRadius);
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

            animation.ChangeAnimation("Pulsing");
        }
        PlayerAudioManager.instance.Push();
    }

    void Start()
    {
        animation = GetComponent<PlayerAnimations>();
    }

    void Update(){
        // Checks for input and the ready state of using either mechanic

        if(!ChargeReady){ chargeTimer -= Time.deltaTime; }
        else if(InputManager.charge.pressed){ Pulse(); }

        if(!PushReady){pushTimer -= Time.deltaTime;}
        else if(InputManager.push.pressed){ Push(); }
    }
}
