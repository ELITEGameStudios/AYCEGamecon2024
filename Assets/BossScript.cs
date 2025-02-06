using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [SerializeField]private int lives = 3, flipDir, flipDirOnDash, shakeFrequency, minDist;
    float playerDist;
    [SerializeField] BossState state;
    [SerializeField] private float timer, stunTime, normalTime, windupTime, hurtTime, explodeWindupTime;
    [SerializeField] private float windupForce, dashSpeed, normalSpeed, shakeIntensity;
    [SerializeField] private Rigidbody2D rb;
    Vector2 stunnedObjectPos;
    
    bool Elapsed {get {return timer <= 0;}}

    enum BossState{
        NORMAL,
        WINDUP,
        DASH,
        EXPLOSIONWINDUP,
        EXPLOSION,
        STUNNED,
        HURT
    }

    // Start is called before the first frame update
    void Awake()
    {
        // state = BossState.NORMAL;
        ReturnToNormal();
    }

    // Update is called once per frame
    void Update()
    {
        if(Elapsed){ChangeState();}
        else{timer -= Time.deltaTime;}
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
                rb.velocity = Vector2.right * flipDir * normalSpeed * Time.fixedDeltaTime + (rb.velocity * Vector2.up);
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
                ReturnToNormal();
                break;
        }
    }

    void Windup(){
        state = BossState.WINDUP;
        Invoke(nameof(Dash), windupTime);
        rb.AddForce(Vector2.left * flipDir * windupForce, ForceMode2D.Impulse);
    }

    void ExplosionWindup(){
        Invoke(nameof(Dash), windupTime);
        rb.AddForce(Vector2.left * flipDir * windupForce, ForceMode2D.Impulse);
    }

    void ReturnToNormal(){
        state = BossState.NORMAL;
        timer = normalTime;
    }

    void Explode(){
        state = BossState.EXPLOSION;
        BossFightManager.Instance.TriggerExplosion();
        Invoke(nameof(Stun), BossFightManager.Instance.ExplosionTime);
    }

    void OnHitMetalBox(GameObject box){
        if(state == BossState.STUNNED){
            lives--;
            Destroy(box);

            if(lives == 0){
                Die();
                return;
            }

            state = BossState.HURT;
            rb.AddForce(Vector2.up * windupForce/2, ForceMode2D.Impulse);
            Invoke(nameof(ReturnToNormal), hurtTime);
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
    }

    void Stun(){
        state = BossState.STUNNED;
        timer = stunTime;
        stunnedObjectPos = transform.position;
    }

    void Dash(){
        state = BossState.DASH;
        flipDirOnDash = flipDir;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "MetalBox"){
            OnHitMetalBox(collision.gameObject);
        }
        else if(collision.gameObject.tag == "BossWall" && state == BossState.DASH){
            OnHitWall();
        }
    }

    void Die(){
        BossFightManager.Instance.OnDeath();
        Destroy(gameObject);
        // just fucking die already
    }
}
