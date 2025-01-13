using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Ground"){
            movement.SetGrounded(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Ground"){
            movement.SetGrounded(false);
        }
    }
}
