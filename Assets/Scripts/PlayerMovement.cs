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
    [SerializeField] private bool grounded;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed, jumpStrength, rocketJumpStrength, rocketJumpTime, rocketJumpTimer, chargeJumpMaxTime, chargeJumpTimer;
    [SerializeField] private FeetScript feet; 

    public float Speed {get { return speed; }}
    public bool Grounded {get { return grounded; }}
    public bool IsRocketJumping {get { return rocketJumpTimer > 0f; }}
    public bool IsCharging {get { return rocketJumpTimer > 0f; }}


    // Update is called once per frame
    void Update()
    {

        if(grounded){
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
        grounded = feet.triggerList.Count > 0; 

    }

    void FixedUpdate(){
        
        velocity = new(InputManager.X, 0);
        transform.position += (Vector3)velocity * Time.fixedDeltaTime * speed;
        
        if(IsRocketJumping){
            rocketJumpTimer -= Time.fixedDeltaTime;
            rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
            
        }   

        if(InputManager.X != 0){
            flipDir = (int)(InputManager.X / Math.Abs(InputManager.X)); 
        }
    }

    void Jump(){
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);        
        rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
        grounded = false;
    }
}
