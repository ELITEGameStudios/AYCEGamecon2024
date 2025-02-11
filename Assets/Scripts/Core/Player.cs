using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerPulse pulse;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D mainCol;

    public PlayerMovement Movement { get { return movement; } }
    public PlayerPulse Pulse { get { return pulse; } }
    public Rigidbody2D Rb {get {return rb;}}
    public  Collider2D MainCol {get {return mainCol;}}
    public static Player main {get; private set;}
    
    public bool dead {get; private set;} = false; 
    public int powerLevel {get; private set;} = 2; // Just represents the amount of mechanics we unlocked, this isnt shown to the user
    public bool unlockedCharge {get {return powerLevel >= 1;}}
    public bool unlockedWallRun {get {return powerLevel >= 1;}}
    public bool unlockedPowerJump {get {return powerLevel >= 2;}}
    
    
    void Awake()
    {
        if(main == null) main = this;
        else if(main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    public void Die(){
        if(dead){return;}

        pulse.enabled = false;
        movement.enabled = false;
        GetComponent<Light2D>().enabled = false;
        dead = true;
        Invoke(nameof(Respawn), 2);
    }

    public void Respawn(){
        if(!dead){return;}
        RespawnSystem.Instance.RespawnPlayer();
        
        pulse.enabled = true;
        movement.enabled = true;
        GetComponent<Light2D>().enabled = true;
        dead = false;
    }

    public void SetUnlock(int newUnlock){
        main.powerLevel = newUnlock;
    }
    
}
