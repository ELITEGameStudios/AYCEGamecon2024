using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerPulse pulse;
    [SerializeField] private Rigidbody2D rb;

    public static Player main {get; private set;}
    public Rigidbody2D Rb {get {return rb;}}
    
    // Start is called before the first frame update
    void Awake()
    {
        if(main == null) main = this;
        else if(main != this) Destroy(this);
    }
    
}
