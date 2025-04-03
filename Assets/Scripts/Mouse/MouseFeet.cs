using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFeet : MonoBehaviour
{
    public bool isGrounded { get; private set; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        isGrounded = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
