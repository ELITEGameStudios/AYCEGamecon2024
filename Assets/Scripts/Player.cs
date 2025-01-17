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
    
    // Start is called before the first frame update
    void Awake()
    {
        if(main == null) main = this;
        else if(main != this) Destroy(this);
    }
    
}
