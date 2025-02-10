using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [SerializeField] private Rigidbody2D physics;
    [SerializeField] private float playerAwakenDistance, signifyTime, timer, resetTimer;
    [SerializeField] private bool kills, resets, collides;

    public enum FallState{
        DEFAULT,
        FALLIMMINENT,
        FALLING,
    }
    public FallState state;

    
    void Awake(){
        state = FallState.DEFAULT;
    }
    
    void Update()
    {
        switch (state){
            case FallState.DEFAULT:
                if(Vector2.Distance(Player.main.gameObject.transform.position, transform.position) <= playerAwakenDistance){
                    state = FallState.FALLIMMINENT; 
                    timer = signifyTime;
                }
                break;
            case FallState.FALLIMMINENT:
                
                if(timer > 0){ timer -= Time.deltaTime; }
                else{
                    state = FallState.FALLING;
                    physics.isKinematic = false;
                } 
                break;
            case FallState.FALLING:
                break;
        } 
    }
    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject == Player.main.gameObject && kills){
            Player.main.Die();
        }
        Destroy(gameObject);
    }
}
