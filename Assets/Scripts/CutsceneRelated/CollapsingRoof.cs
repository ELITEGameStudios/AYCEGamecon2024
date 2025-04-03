using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingRoof : MonoBehaviour
{
    [SerializeField] private List<Rigidbody2D> fallingObjects = new List<Rigidbody2D>();
    [SerializeField] private List<Rigidbody2D> objectsWithColliders = new List<Rigidbody2D>();
    [SerializeField] private List<Rigidbody2D> destroyObjects = new List<Rigidbody2D>();
    [SerializeField] private float disableCollisionsTime = 0.5f;
    private bool collapsed = false;

    [SerializeField] private Transform explosionPoint;
    [SerializeField] private float explosionForce = 5.0f;
    [SerializeField] private float explosionTorque = 5.0f;
    [SerializeField] private float gScale = 1.2f;

    private void Start()
    {
        // Ensure all objects start as Static
        foreach (Rigidbody2D rb in fallingObjects)
        {
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collapsed && collision.gameObject.CompareTag("Player"))
        {
            collapsed = true;
            StartCoroutine(ActivateExplosion());
        }
    }

    private IEnumerator DisableColliders()
    {
        yield return new WaitForSeconds(disableCollisionsTime);

        foreach (Rigidbody2D rb in objectsWithColliders)
        {
            if (rb != null)
            {
                Collider2D col = rb.GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = false;
            }
        }

        foreach (Rigidbody2D rb in destroyObjects)
            Destroy(rb.gameObject);
    }

    private IEnumerator ActivateExplosion()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (Rigidbody2D rb in fallingObjects)
        {
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // Enable physics
                rb.gravityScale = gScale; // Enable gravity

                // Calculate direction away from explosion point
                Vector2 explosionDirection = (rb.position - (Vector2)explosionPoint.position).normalized;

                // Add slight upward force for a better explosion effect
                explosionDirection += Vector2.up * Random.Range(0.2f, 0.5f);
                explosionDirection.Normalize();

                rb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-explosionTorque, explosionTorque), ForceMode2D.Impulse);
            }
        }

        StartCoroutine(DisableColliders());
    }
}
