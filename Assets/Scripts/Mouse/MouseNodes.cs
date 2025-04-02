using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseNodes : MonoBehaviour
{
    private bool passed = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MouseMovement mouse = FindObjectOfType<MouseMovement>();

            if (mouse != null)
                mouse.PassedNode(transform);

            passed = true;
        }
    }
}
