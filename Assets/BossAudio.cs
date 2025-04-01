using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    [SerializeField] private float volume = 0.5f;
    
    public AudioClip 
        chargeUp,
        stunned,
        scream,
        takeDamage,
        explosion,
        smash,
        dash,
        footsteps,
        death;

    public float 
        chargeUpLevel,
        stunnedLevel,
        screamLevel,
        takeDamageLevel,
        explosionLevel,
        smashLevel,
        dashLevel,
        footstepsLevel;

    [SerializeField] private AudioSource mainSource, footstepSource;

    void Awake(){
        // volume = 0.5f;
        mainSource.volume = volume;
    }

    /*
        Boss Scream
        Boss Dash
        Boss Wall Slam + Explosion
        Boss Charge up (5 sounds)
        Boss Charged Explosion
        Boss Stunned Sound
        Boss Takes Damage (Goes into Scream)

    */
    public void Enable(){
        // AudioSource.PlayClipAtPoint(chargeUp, transform.position, volume);
    }

    public void ChargeUp(){
        UseMain(chargeUp);
    }

    public void Stun(){
        AudioSource.PlayClipAtPoint(stunned, Camera.main.transform.position, volume);
    }

    public void Scream(){
        AudioSource.PlayClipAtPoint(scream, Camera.main.transform.position, volume * screamLevel);
    }

    public void TakeDamage(){
        AudioSource.PlayClipAtPoint(takeDamage, Camera.main.transform.position, volume);
    }

    public void Explode(){
        AudioSource.PlayClipAtPoint(explosion, Camera.main.transform.position, volume);
    }

    public void Smash(){
        AudioSource.PlayClipAtPoint(smash, Camera.main.transform.position, volume);
    }

    public void Dash(){
        AudioSource.PlayClipAtPoint(dash, Camera.main.transform.position, volume);
    }

    public void Die(){
        AudioSource.PlayClipAtPoint(death, Camera.main.transform.position, volume);
    }
    
    public void UseMain(AudioClip clip, float volume = -1){
        if(mainSource.isPlaying) mainSource.Stop();
        if(volume == -1){mainSource.volume = this.volume;}
        else {mainSource.volume = volume;}
        mainSource.clip = clip;
        mainSource.Play();
    }

    public void StopMain(){
        mainSource.Stop();
    }

    public void Footstep(){
        if(footstepSource.isPlaying) footstepSource.Stop();
        footstepSource.volume = this.volume;
        footstepSource.clip = footsteps;
        footstepSource.Play();
    }

    public void StopFootstep(){
        footstepSource.Stop();
    }
}
