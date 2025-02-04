using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHasardObject : MonoBehaviour
{
    public bool active;
    GameObject effect;
    void OnCollisionEnter2D(Collision2D col){
        if(active && col.gameObject.layer == LayerMask.NameToLayer("Environment")){
            if(effect != null){
                GameObject clone = Instantiate(effect, transform);
                clone.transform.SetParent(null);
            }
            Destroy(gameObject);    
        }
    }

    public void Activate(){
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        active = true;
    }
}
