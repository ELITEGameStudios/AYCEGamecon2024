using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector2 velocity { get; private set; }
    public float directionX { get; private set; }
    [SerializeField] private bool grounded, hasRocketJump;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed, jumpStrength, rocketJumpStrength, rocketJumpTime, rocketJumpTimer;
    [SerializeField] private Collider2D feetCollider; 

    public float Speed {get { return speed; }}
    public bool Grounded {get { return grounded; }}
    public bool HasRocketJump {get { return grounded; }}
    public bool IsRocketJumping {get { return rocketJumpTimer > 0f; }}


    // Update is called once per frame
    void Update()
    {
        directionX = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space)) { 
            if(grounded) Jump();
            else if(hasRocketJump && !IsRocketJumping) RocketJump();
        }


    }

    public void SetGrounded(bool ground){
        grounded = ground;
        hasRocketJump = ground || hasRocketJump;
    }

    void FixedUpdate(){
        
        velocity = new(directionX, 0);
        transform.position += (Vector3)velocity * Time.fixedDeltaTime * speed;
        
        if(IsRocketJumping){
            rocketJumpTimer -= Time.fixedDeltaTime;
            rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
            Debug.Log("executing");
        }
        
    }

    void Jump(){
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
        grounded = false;
    }

    void RocketJump(){
        rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
        rocketJumpTimer = rocketJumpTime;
        hasRocketJump = false;

    }
}
