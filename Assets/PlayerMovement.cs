using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Vector2 velocity { get; private set; }
    public float directionX { get; private set; }
    [SerializeField] private bool grounded;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed, jumpStrength;
    [SerializeField] private Collider2D feetCollider; 

    public float Speed {get { return speed; }}
    public bool Grounded {get { return grounded; }}


    // Update is called once per frame
    void Update()
    {
        directionX = Input.GetAxis("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space) && grounded){
            Jump();
        }
    }

    public void SetGrounded(bool ground){
        grounded = ground;
    }

    void FixedUpdate(){
        velocity = new Vector2(directionX, 0);
        transform.position += (Vector3)velocity * Time.fixedDeltaTime * speed;
        
    }

    void Jump(){
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
        grounded = false;

    }
}
