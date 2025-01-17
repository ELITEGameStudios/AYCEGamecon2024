using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetScript : MonoBehaviour
{
    public List<Collider2D> triggerList;
    public List<Collider2D> collisionList;
    public Collider2D playerCol;
    [SerializeField] private PlayerMovement movement;

    void Awake(){
        triggerList ??= new();
        collisionList ??= new();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other != playerCol && !other.isTrigger){
            triggerList.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        triggerList.Remove(other);
    }
}
