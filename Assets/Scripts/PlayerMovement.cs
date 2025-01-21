using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector2 velocity { get; private set; }
    public float directionX { get; private set; }
    public int flipDir { get; private set; } = 1;
    public int flipDirRaw { get; private set; } = 1;
    [SerializeField] private bool grounded;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed, jumpStrength, rocketJumpStrength, rocketJumpTime, rocketJumpTimer, chargeJumpMaxTime, chargeJumpTimer, magTime, magTimer, initGravScale;
    [SerializeField] private FeetScript feet; 

    public float Speed {get { return speed; }}
    public bool Grounded {get { return grounded; }}
    public bool IsRocketJumping {get { return rocketJumpTimer > 0f; }}
    public bool IsCharging {get { return rocketJumpTimer > 0f; }}
    public bool IsLedged {get { return ledge != null; }}

    public bool isOnWall;
    public LedgeScript ledge;
    public Collider2D wallCol;

    public Vector2 magnetizeStartPoint;
    public Vector2 magnetizeEndPoint;
    public enum PlayerMoveState{
        ONGROUND,
        OFFGROUND,
        MAGNETIZING,
        CLIMBING
    }

    public PlayerMoveState moveState;

    void Start(){
        initGravScale = Player.main.Rb.gravityScale;
    }


    // Update is called once per frame
    void Update()
    {

        if(grounded || moveState == PlayerMoveState.CLIMBING){
            if(InputManager.jump.pressed){
                chargeJumpTimer += Time.deltaTime;
            }
            else if(InputManager.jump.releasedThisFrame){
                rocketJumpTimer = Mathf.Clamp(chargeJumpTimer, 0, chargeJumpMaxTime) - InputManager.jumpInputThreshold;
                chargeJumpTimer = 0;
                Jump();
            }
        }
        else{  chargeJumpTimer = 0;  }
        grounded = feet.triggerList.Count > 0 || IsLedged; 

        if(moveState != PlayerMoveState.CLIMBING && moveState != PlayerMoveState.MAGNETIZING){
            moveState = grounded ? PlayerMoveState.ONGROUND : PlayerMoveState.OFFGROUND;
        }

    }

    void FixedUpdate(){

        if(moveState == PlayerMoveState.CLIMBING){ velocity = new(0, InputManager.X * flipDir); }
        else{ velocity = new(InputManager.X, 0); }    
        Player.main.Rb.gravityScale = moveState == PlayerMoveState.CLIMBING ? 0 : initGravScale;

        if(!IsLedged)
            { transform.position += (Vector3)velocity * Time.fixedDeltaTime * speed; }
        else
        { 
            transform.position = (Vector2)ledge.transform.position + ledge.offset; 
            rb.velocity = Vector2.zero;
        }
        Debug.Log(IsLedged);
        
        if(IsRocketJumping){
            rocketJumpTimer -= Time.fixedDeltaTime;
            rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }   

        if(moveState != PlayerMoveState.CLIMBING && moveState != PlayerMoveState.MAGNETIZING){
            if(InputManager.X != 0){
                flipDir = (int)(InputManager.X / Math.Abs(InputManager.X)); 
            }
            if(InputManager.XRaw != 0){
                flipDirRaw = (int)(InputManager.XRaw / Math.Abs(InputManager.XRaw)); 
            }
        }
        

        if(moveState == PlayerMoveState.MAGNETIZING){
            transform.position = Vector2.Lerp(magnetizeEndPoint, magnetizeStartPoint, magTimer / magTime );
            Player.main.Rb.velocity = Vector2.zero;

            if(magTimer <= 0){
                moveState = PlayerMoveState.CLIMBING;

            }
            else{
                magTimer -= Time.fixedDeltaTime;
            }
        }

        if(moveState == PlayerMoveState.CLIMBING){
            transform.position = new Vector2(
                wallCol.ClosestPoint(transform.position).x + (wallCol.transform.localScale.x / 2) * flipDir,
                Mathf.Clamp(transform.position.y, 
                    wallCol.transform.position.y - wallCol.transform.localScale.y/2 + transform.localScale.y/2,
                    wallCol.transform.position.y + wallCol.transform.localScale.y/2 - transform.localScale.y/2
                )
            );
        }
    }

    void Jump(){
        ledge = null;
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);        
        rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
        grounded = false;
        moveState = PlayerMoveState.OFFGROUND;
    }

    public void SetLedge(LedgeScript ledge){
        if(ledge == null){return;}
        this.ledge = ledge;

    }

    public void Magnetize(Collider2D col){
        moveState = PlayerMoveState.MAGNETIZING;

        wallCol = col;
        magTimer = magTime;
        magnetizeEndPoint = (Vector2)wallCol.ClosestPoint(transform.position) + Vector2.right * ((Vector2)wallCol.transform.localScale / 2) * flipDir;
        // magnetizeEndPoint = (Vector2)wallCol.ClosestPoint(transform.position);
    }
}
