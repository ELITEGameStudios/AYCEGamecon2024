using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public bool kills;
    [SerializeField] private GameObject effect;
    void OnCollisionEnter2D(Collision2D col){
        if(col.collider == Player.main.MainCol){ 
            if(effect != null){
                GameObject clone = Instantiate(effect, transform);
                clone.transform.SetParent(null);
                clone.transform.localScale = Vector3.one;
                clone.transform.position = transform.position;

                // Literally just copypasted from player push code to make explosion effect
                // Gets all colliders in the radius of the push
                Collider2D[] colliders = Physics2D.OverlapCircleAll(Player.main.transform.position, 1);
                foreach (Collider2D miniCol in colliders)
                {
                    try{
                        if(miniCol.attachedRigidbody == Player.main.Rb){continue;}

                        // Applies an explosion force to a dynamic rigidbody relative to the players position
                        if(miniCol.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic )
                        { 
                            Vector2 closestPos = miniCol.ClosestPoint((Vector2)transform.position);
                            Vector2 directionVector = (closestPos - (Vector2)transform.position).normalized;
                            float distance = Vector2.Distance(closestPos, miniCol.transform.position);

                            miniCol.attachedRigidbody.AddForceAtPosition(
                                // This formula just multiplies the direction of the force by the force coefficient 
                                // Then, this gets scaled by how close it is to the player. if the object is close to player, it gets most force.
                                directionVector * 4 * (1 - distance) / 1, 
                                transform.position,
                                ForceMode2D.Impulse 
                            );
                        }
                    }
                    catch (System.NullReferenceException) { continue; } // Skips any errors based on if the collider has a rigidbody or not. 
                }

            }
            Destroy(gameObject);    
        }
    }

    
}
