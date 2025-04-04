using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private Light2D feetLight, pulseLightX; 
    private float playerSide;

    private PlayerAnimations animations;
    [SerializeField] private SpriteRenderer[] spriteList;
    public SpriteRenderer[] SpriteList {get{return spriteList;}}

    public float Speed {get { return speed; }}
    public bool Grounded {get { return grounded; }}
    public bool IsWalking {get { return Mathf.Abs(velocity.x) > 0.7f && moveState == PlayerMoveState.ONGROUND && !IsCrouched && !IsLedged; }}
    public bool IsRocketJumping {get { return rocketJumpTimer > 0f; }}
    public bool IsCharging {get { return rocketJumpTimer > 0f; }}
    public bool IsLedged {get { return ledge != null; }}
    public bool IsCrouched {get { return InputManager.crouch.pressed && grounded; }}

    public bool IsFlipped { get { return flipDir > 0; } }

    [SerializeField] 
    public bool collidingWithSticky {
        get { 
            foreach (Collider2D col in feet.triggerList){
                if(col.tag == "Sticky") return true;
            }    
            return false; 
        }
    }

    public bool isOnWall;
    public LedgeScript ledge;
    public Collider2D wallCol;

    public Vector2 magnetizeStartPoint;
    public Vector2 magnetizeEndPoint;
    public enum PlayerMoveState{
        ONGROUND,
        OFFGROUND,
        MAGNETIZING,
        CLIMBING,
        OFFWALL
    }

    public PlayerMoveState moveState;

    void Start(){
        initGravScale = Player.main.Rb.gravityScale;
        animations = GetComponent<PlayerAnimations>();
    }

    void OnDisable(){
        Player.main.Rb.gravityScale = initGravScale;
        moveState = PlayerMoveState.OFFGROUND;
        ledge = null;
        rocketJumpTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        pulseLightX.enabled = Player.main.unlockedCharge;
        feetLight.enabled = Player.main.unlockedPowerJump;

        if(grounded || moveState == PlayerMoveState.CLIMBING){
            if(InputManager.crouch.pressed)
            { 
                chargeJumpTimer += Time.deltaTime;
                if (moveState == PlayerMoveState.CLIMBING)
                    animations.ChangeAnimation("WallJumpPowering");
                else
                    animations.ChangeAnimation("JumpPowering");
                if(chargeJumpTimer > InputManager.jumpInputThreshold && Player.main.powerLevel >= 2 && !PlayerAudioManager.instance.Charging()){
                    PlayerAudioManager.instance.WindupChargeStart();
                }
            }
            else if(InputManager.crouch.releasedThisFrame){ 
                chargeJumpTimer = 0; 
                PlayerAudioManager.instance.StopCharge();
            }
            if(InputManager.jump.pressedThisFrame){
                rocketJumpTimer = Player.main.powerLevel >= 2 ? Mathf.Clamp(chargeJumpTimer, 0, chargeJumpMaxTime) - InputManager.jumpInputThreshold : 0;
                chargeJumpTimer = 0;
                Jump();
            }
        }
        else{  chargeJumpTimer = 0;  }
        grounded = feet.triggerList.Count > 0 || IsLedged; 

        if(moveState != PlayerMoveState.CLIMBING && moveState != PlayerMoveState.MAGNETIZING){
            moveState = grounded ? PlayerMoveState.ONGROUND : PlayerMoveState.OFFGROUND;
        }

        if (moveState == PlayerMoveState.OFFGROUND && moveState != PlayerMoveState.CLIMBING)
        {
            animations.ChangeAnimation("Falling");
        }

        if (InputManager.Y <= -0.1f && IsLedged){
            ledge = null;
        }

        UpdateVVSpriteFlip();
        CheckAnimations();

    }

    void UpdateVVSpriteFlip(){
        if(IsLedged){return;}

        foreach (SpriteRenderer sprite in spriteList)
        {
            if (velocity.x > 0)
                sprite.flipX = true;
            else if (velocity.x < 0)
                sprite.flipX = false;
        }
    }

    void UpdateVVSpriteFlip(bool left){
        if(IsLedged){return;}

        foreach (SpriteRenderer sprite in spriteList)
        {
            sprite.flipX = left;
        }
    }

    void FixedUpdate(){
        // Debug.Log(flipDirRaw);

        if(moveState == PlayerMoveState.CLIMBING)
        { 
            velocity = new(0, -Mathf.Clamp(InputManager.X - (InputManager.Y > 0 ? InputManager.Y : 0), -1, 1) * playerSide); 
        }
        else
        { 
            velocity = new(InputManager.X, 0);
        }    
        
        if(!collidingWithSticky){ Player.main.Rb.gravityScale = moveState == PlayerMoveState.CLIMBING ? 0 : initGravScale; }
        else{ Player.main.Rb.gravityScale = 15; }

        if(!IsLedged && !IsCrouched) {
            transform.position += (Vector3)velocity * Time.fixedDeltaTime * speed; 
            
        }
        else
        { 
            transform.position = (Vector2)ledge.transform.position + ledge.offset; 
            rb.velocity = Vector2.zero;
            animations.ChangeAnimation("Ledging");
        }
        // Debug.Log(IsLedged);
        
        if(IsRocketJumping){
            rocketJumpTimer -= Time.fixedDeltaTime;
            rb.AddForce(
                ( moveState == PlayerMoveState.OFFWALL ? Vector2.right * playerSide :  Vector2.up) 
                * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }   

        if(
            moveState != PlayerMoveState.CLIMBING 
            && moveState != PlayerMoveState.MAGNETIZING 
            && !IsLedged
        ){
            if(InputManager.X != 0){
                flipDir = (int)(InputManager.X / Math.Abs(InputManager.X)); 
            }
            if(InputManager.XRaw != 0){
                flipDirRaw = (int)(InputManager.XRaw / Math.Abs(InputManager.XRaw)); 
            }
        }
        

        if(moveState == PlayerMoveState.MAGNETIZING){
            transform.position = Vector2.Lerp( magnetizeStartPoint, magnetizeEndPoint, MathF.Pow(10f,  1.5f*(1 - (magTimer / magTime)) - 1.4786f) - 0.035f);
            Player.main.Rb.velocity = Vector2.zero;

            if(magTimer <= 0){
                moveState = PlayerMoveState.CLIMBING;
                // animations.ChangeAnimation("ClimbIdle");
                transform.position = magnetizeEndPoint;

            }
            else{
                magTimer -= Time.fixedDeltaTime;
            }
        }

        if(moveState == PlayerMoveState.CLIMBING){
            transform.position = new Vector2(
                magnetizeEndPoint.x,
                Mathf.Clamp(transform.position.y, 
                    wallCol.transform.position.y - wallCol.transform.localScale.y/2 + transform.localScale.y/2,
                    Mathf.Infinity
                )
            );

            if(transform.position.y >=wallCol.transform.position.y + wallCol.transform.localScale.y/2 - transform.localScale.y/2){
                moveState = PlayerMoveState.OFFWALL;
                Jump();
            }

            else if(transform.position.y <= wallCol.transform.position.y - wallCol.transform.localScale.y/2 + transform.localScale.y/2){
                moveState = PlayerMoveState.OFFWALL;
            }
        }
    }

    public void CheckAnimations()
    {
        if (PlayerAnimations.currentAnimation == "Jumping" || PlayerAnimations.currentAnimation == "Landing" 
            || PlayerAnimations.currentAnimation == "Pulsing" || PlayerAnimations.currentAnimation == "Charging"
            || PlayerAnimations.currentAnimation == "Ledging")
            return;

        if (PlayerAnimations.currentAnimation == "Falling")
        {
            if (Grounded)
                animations.ChangeAnimation("Landing");

            return;
        }

        if (moveState == PlayerMoveState.CLIMBING)
        {
            if (velocity != Vector2.zero)
            {
                animations.ChangeAnimation("Climbing");
            }
            else if (PlayerAnimations.currentAnimation == "WallJumpPowering")
            {
                if (IsRocketJumping)
                    animations.ChangeAnimation("Jumping");
            }
            else
            {
                animations.ChangeAnimation("ClimbIdle");
            }

            return;
        }

        if (PlayerAnimations.currentAnimation == "JumpPowering" && InputManager.crouch.pressed)
        {
            if (IsRocketJumping)
                animations.ChangeAnimation("Jumping");
            return;
        }   

        if (velocity != Vector2.zero)
            animations.ChangeAnimation("Running");
        else
            animations.ChangeAnimation("Idle");
    }

    void Jump(){
        ledge = null;
        if(moveState == PlayerMoveState.CLIMBING){
            rb.AddForce(
                Vector2.right * playerSide * jumpStrength * 1.5f
                + Vector2.up * jumpStrength / 2, ForceMode2D.Impulse);
            moveState = PlayerMoveState.OFFWALL;
            // Debug.Log(Vector2.right * playerSide * jumpStrength * 3 + "\n PlayerSide: " + playerSide);
        }
        else{
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);        
            rb.AddForce(Vector2.up * rocketJumpStrength * Time.fixedDeltaTime, ForceMode2D.Impulse);
            moveState = PlayerMoveState.OFFGROUND;
        }
        grounded = false;
        PlayerAudioManager.instance.TriggerJumpSFX();
        if(IsRocketJumping){
            PlayerAudioManager.instance.ReleaseCharge();
        }

        animations.ChangeAnimation("Jumping");
    }

    public void SetLedge(LedgeScript ledge){
        if(ledge == null){return;}
        
        UpdateVVSpriteFlip(ledge.Left);
        this.ledge = ledge;

    }

    public void Magnetize(Collider2D col){
        moveState = PlayerMoveState.MAGNETIZING;
        wallCol = col;
        magTimer = magTime;

        Vector2 pointClose = (Vector2)wallCol.ClosestPoint(transform.position);
        
        playerSide = ((Vector2)transform.position - pointClose).x / Mathf.Abs(((Vector2)transform.position - pointClose).x);
        Debug.Log(playerSide);
        magnetizeEndPoint = pointClose + Vector2.right * ((Vector2)wallCol.transform.localScale / 2) * playerSide;
        magnetizeStartPoint = transform.position;
        // magnetizeEndPoint = (Vector2)wallCol.ClosestPoint(transform.position);
    }
}
