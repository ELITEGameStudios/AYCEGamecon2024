using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherCollider : MonoBehaviour
{
    public List<Collider2D> triggerList;
    public List<Collider2D> collisionList;
    [SerializeField] private bool hasPlayer, hasEnvironment;
    [SerializeField] public bool IsCrushingPlayer {get{return hasPlayer && hasEnvironment;}}
    
    void Awake(){
        triggerList ??= new();
        collisionList ??= new();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other == Player.main.MainCol){
            triggerList.Add(other);
            hasPlayer = true;
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Environment")){
            triggerList.Add(other);
            hasEnvironment = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other == Player.main.MainCol){
            triggerList.Remove(other);
            hasPlayer = false;
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Environment")){
            triggerList.Remove(other);
            hasEnvironment = false;
        }
    }
}
