using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHasardObject : MonoBehaviour
{
    public bool active, destroys, kills;
    [SerializeField] private GameObject effect;
    void OnCollisionEnter2D(Collision2D col){
        if(active){
            if(col.gameObject.layer == LayerMask.NameToLayer("Environment")){
                if(effect != null){
                    GameObject clone = Instantiate(effect, transform);
                    clone.transform.SetParent(null);
                }
                if(destroys) {Destroy(gameObject);};    
            }

            else if(col.collider == Player.main.MainCol && kills){
                Player.main.Die(); 
            }
        }
    }

    public void Activate(){
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        active = true;
    }
}
