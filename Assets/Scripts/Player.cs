using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public int powerLevel {get; private set;} = 0; // Just represents the amount of mechanics we unlocked, this isnt shown to the user
    public bool unlockedCharge {get {return powerLevel >= 1;}}
    public bool unlockedWallRun {get {return powerLevel >= 1;}}
    public bool unlockedPowerJump {get {return powerLevel >= 2;}}
    
    
    void Awake()
    {
        if(main == null) main = this;
        else if(main != this) Destroy(this);
    }
    

    public void SetUnlock(int newUnlock){
        main.powerLevel = newUnlock;
    }
    
}
