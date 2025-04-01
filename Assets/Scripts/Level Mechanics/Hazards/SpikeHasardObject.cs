using System.Collections;
using System.Collections.Generic;
using SoundSystems;
using UnityEngine;

public class SpikeHasardObject : MonoBehaviour
{
    public bool active, destroys, kills, collides, colliderInitEnabled;
    [SerializeField] private GameObject effect;
    [SerializeField] private EnvironmentalSound fallSound;
    
    void Awake(){
        colliderInitEnabled = GetComponent<Collider2D>().enabled;
    }
    void OnCollisionEnter2D(Collision2D col){
        if(active && collides){
            if(col.gameObject.layer == LayerMask.NameToLayer("Environment")){
                if(effect != null){
                    GameObject clone = Instantiate(effect, transform);
                    clone.transform.SetParent(null);
                }
                if(destroys) {

                    if(fallSound != null){
                        // EnvironmentalSoundSystem.instance.FindEnvSound("spike_fall").Stop();
                    }

                    Destroy(gameObject);
                };    
            }

            else if(col.collider == Player.main.MainCol && kills){
                Player.main.Die(); 
            }
        }
    }

    public void Activate(){
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Collider2D>().enabled = collides;
        active = true;
        if(fallSound != null){
            EnvironmentalSoundSystem.instance.CreateEnvSound("spike_fall", fallSound, soundOrgin: gameObject);
        }
    }

    public void Deactivate(){
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = colliderInitEnabled;
        active = false;
        if(fallSound != null){
            // EnvironmentalSoundSystem.instance.FindEnvSound("spike_fall").Stop();
        }
    }
}
