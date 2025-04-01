using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField]private int lives = 3, flipDirOnDash, shakeFrequency, minDist;
    public int flipDir;
    float playerDist;
    [SerializeField] BossState state;
    [SerializeField] private float timer, stunTime, normalTime, windupTime, hurtTime, explodeWindupTime;
    [SerializeField] private float windupForce, dashSpeed, normalSpeed, shakeIntensity;
    [SerializeField] private Rigidbody2D rb;
    Vector2 stunnedObjectPos;

    [Header("For walking animation sync")]
    [SerializeField] private AnimationCurve walkRate;
    [SerializeField] private int walkFrameCurrent;
    [SerializeField] private float walkFrameTimer;
    [SerializeField] private float[] walkFrames;
    [SerializeField] private float walkVelocity;
    [SerializeField] private bool footSoundPlayed;
    
    [Header("Animator Sprites")]
    [SerializeField] private SpriteRenderer animatedLights;
    [SerializeField] private SpriteRenderer animatedMain;
    
    [Header("Setup boss related scripts")]
    [SerializeField] private BossAnimationScript animScript;
    [SerializeField] private BossAudio audio;
    [SerializeField] private ExplosionScript expScript;

    [Header("Animation Required properties")]
    public float dashTiltAngle;
    public float windupTilt, shutdownTilt, explosionTilt;
    public float dashTiltTime, shutdownTiltTime;
    public AnimationCurve dashCurve, explosionCurve, shutdownCurve, restartCurve;
    public Color dashColor, normalColor, shutdownColor, awakenColor;
    
    bool Elapsed {get {return timer <= 0;}}
    bool Flipped {get {return flipDir < 0;}}

    enum BossState{
        NORMAL,
        WINDUP,
        DASH,
        EXPLOSIONWINDUP,
        EXPLOSION,
        STUNNED,
        HURT,
        INACTIVE
    }

    // Start is called before the first frame update
    void Awake()
    {
        // state = BossState.NORMAL;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == BossState.INACTIVE){return;}
        else if(state == BossState.NORMAL){
            animatedLights.flipX = Flipped;
            animatedMain.flipX = Flipped;
        }

        if(Elapsed){
            ChangeState();
            audio.StopFootstep();
        }
        else{
            timer -= Time.deltaTime;
            // walkFrameTimer = walkFrameTimer < 1 ? walkFrameTimer + Time.deltaTime : walkFrameTimer - 1 + Time.deltaTime;
            walkFrameTimer += Time.deltaTime;
            walkFrameCurrent = (int)(walkFrameTimer*walkFrames.Length);
            // if(walkFrameCurrent == 12 || walkFrameCurrent == 0){
            //     if(footSoundPlayed){return;}
            //     footSoundPlayed = true;
            // }
            // else{footSoundPlayed = false;}

            try { walkVelocity = walkFrames[walkFrameCurrent]; }
            catch { 
                walkFrameTimer = walkFrameTimer - 1 + Time.deltaTime;
                walkFrameCurrent = (int)(walkFrameTimer*walkFrames.Length);
                walkVelocity = walkFrames[walkFrameCurrent];
            }
        }


    }

    public void ActivateRobot(){
        // state = BossState.NORMAL;
        Invoke(nameof(ReturnToNormal), hurtTime);
        animScript.SetColor(normalColor, 0);
        animScript.SetOscilation(6, 0);
        animScript.TiltHead(0, restartCurve, hurtTime);
        audio.Enable();
        
    }

    void FixedUpdate(){
        playerDist = Vector2.Distance(transform.position, Player.main.transform.position);
        Vector2 playerOffset = Player.main.transform.position - transform.position;
        flipDir = (int)(playerOffset.x / Mathf.Abs(playerOffset.x)); 

        if(playerDist <= minDist && state == BossState.NORMAL){
            ChangeState();
        }


        switch(state){
            case BossState.NORMAL:
                rb.velocity = (Vector2.right * flipDir * normalSpeed * Time.fixedDeltaTime + (rb.velocity * Vector2.up)) * walkVelocity;
                break;
            case BossState.DASH:
                rb.velocity = Vector2.right * flipDirOnDash * dashSpeed * Time.fixedDeltaTime + (rb.velocity * Vector2.up);
                break;
            case BossState.STUNNED:
                transform.position = 
                    Vector2.Lerp( 
                        stunnedObjectPos + Vector2.right * shakeIntensity, 
                        stunnedObjectPos + Vector2.left * shakeIntensity,
                        Mathf.Sin(2*Mathf.PI * shakeFrequency * timer)
                    );
                break;
        }
    }

    void ChangeState(){
        switch(state){
            case BossState.NORMAL:
                Windup();
                break;
            case BossState.STUNNED:
                ActivateRobot();
                break;
        }
    }

    void Windup(){
        state = BossState.WINDUP;
        Invoke(nameof(Dash), windupTime);
        rb.AddForce(Vector2.left * flipDir * windupForce, ForceMode2D.Impulse);

        animScript.SetModel(true);
        animScript.TiltHead(dashTiltAngle, dashCurve, windupTime);
        animScript.SetColor(Color.clear, windupTime);
        animScript.DashWarn();
        audio.Scream();
    }

    void ExplosionWindup(){
        Invoke(nameof(Dash), windupTime);
        rb.AddForce(Vector2.left * flipDir * windupForce, ForceMode2D.Impulse);
        
    }

    void ReturnToNormal(){
        state = BossState.NORMAL;
        timer = normalTime;
        walkFrameTimer = 0;
        animScript.SetColor(normalColor, 1);
        animScript.SetModel(false);
        animScript.SetOscilation(0, 0);
        audio.Footstep();
    }

    void Explode(){
        state = BossState.EXPLOSION;
        BossFightManager.Instance.TriggerExplosion();
        Invoke(nameof(Stun), BossFightManager.Instance.ExplosionTime);

        animScript.SetOscilation(5, 0);
        animScript.TiltHead(explosionTilt, explosionCurve, BossFightManager.Instance.ExplosionTime + 0.2f);
        animScript.ExplosionBoom();
        audio.StopMain();
        audio.Explode();
    
    }

    void OnHitMetalBox(GameObject box){
        audio.Smash();
        if(state == BossState.STUNNED){
            lives--;
            Destroy(box);

            if(lives == 0){
                Die();
                return;
            }

            audio.TakeDamage();
            state = BossState.HURT;
            rb.AddForce(Vector2.up * windupForce/2, ForceMode2D.Impulse);
            
            ActivateRobot();
        }
        else{
            // break metal box
            Destroy(box);
        }
    }

    void OnHitWall(){
        state = BossState.EXPLOSIONWINDUP;
        Invoke(nameof(Explode), explodeWindupTime);
        timer = stunTime;

        audio.ChargeUp();
        animScript.SetColor(Color.red, 0);
        animScript.SetOscilation(5, explodeWindupTime);
        animScript.TiltHead(windupTilt, AnimationCurve.EaseInOut(0, 0, 1, 1), explodeWindupTime);
        animScript.SetPose(false);
        expScript.gameObject.SetActive(true);
        expScript.WarnExplosion(explodeWindupTime);
        
    }

    void Stun(){
        state = BossState.STUNNED;
        timer = stunTime;
        stunnedObjectPos = transform.position;
        
        animScript.SetColor(Color.clear, shutdownTiltTime);
        animScript.SetOscilation(0, 0);
        animScript.TiltHead(shutdownTilt, shutdownCurve, shutdownTiltTime);
        audio.Stun();
    }

    void Dash(){
        state = BossState.DASH;
        flipDirOnDash = flipDir;

        animScript.SetColor(Color.white, 0);
        animScript.SetOscilation(0, 0);
        animScript.SetPose(true);
        audio.Dash();
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject == Player.main.gameObject){ Player.main.Die(); }

        if(collision.gameObject.tag == "MetalBox"){
            OnHitMetalBox(collision.gameObject);
        }
        else if(collision.gameObject.tag == "BossWall" && state == BossState.DASH){
            OnHitWall();
        }
    }

    void Die(){
        BossFightManager.Instance.OnDeath();
        animScript.DeathAnimation();
        audio.Die();
        Destroy(gameObject);
        // just fucking die already
    }
}
