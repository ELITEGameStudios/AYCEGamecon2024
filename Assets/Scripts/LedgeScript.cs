using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeScript : MonoBehaviour
{
    public Vector2 offset {get; private set;} = new Vector2(0.75f, -0.25f); // Defult offset of the player to the centre of the ledge object
    [SerializeField] private bool left; // flips the offset based on the intended direction
    [SerializeField] private bool ready {get {return timer <= 0;}}
    [SerializeField] private float cooldown = 0.5f, timer;

    // Start is called before the first frame update
    void Awake()
    {
        if(left){ offset *= Vector2.left; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!ready){timer -= Time.fixedDeltaTime; }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other == Player.main.MainCol && !Player.main.Movement.IsLedged && ready){
            Player.main.Movement.SetLedge(this);
            // timer = cooldown;
        }
    }
}
