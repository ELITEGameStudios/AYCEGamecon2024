using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource chargeSource;
    // [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource[] sourcesList;

    public static PlayerAudioManager instance {get; private set;}

    [SerializeField] private AudioClip pulseClip;
    [SerializeField] private AudioClip footstepsClip, labFootstepsClip;
    [SerializeField] private AudioClip jumpClip, chargeClip, chargeReleaseClip;
    [SerializeField] private AudioClip pushClip, deathClip;
    
    [SerializeField] private float 
        walkingFadeConstant, 
        walkAudiolevel = 1, 
        labWalkAudioLevel = 1, 
        pushAudioLevel = 1, 
        pulseAudioLevel = 1, 
        jumpAudioLevel = 1, 
        chargeAudioLevel = 1, 
        chargeReleaseAudioLevel = 1,
        deathAudioLevel = 1;


    public bool isLabFootsteps {get{return walkSource.clip == labFootstepsClip;}}

    bool footsteps;
    [SerializeField] private bool charging;

    void Awake() {
        if(instance == null)instance = this;
        else if(instance != this) Destroy(gameObject);
        sourcesList = new[]{mainSource};
    }

    void Start() {
        mainSource = Player.main.Audio;
        walkSource.clip = footstepsClip;
        footsteps = false;
    }

    public void SetLabFootsteps(){
        footsteps = false;
        walkSource.clip = labFootstepsClip;
        walkSource.Stop();
    }

    public void SetCaveFootsteps(){
        footsteps = false;
        walkSource.clip = footstepsClip;
        walkSource.Stop();
    }

    void Update() {
        foreach (AudioSource source in sourcesList) { source.volume = AudioSystem.volume; }
        float currentWalkLevel = isLabFootsteps ? labWalkAudioLevel : walkAudiolevel;
        walkSource.volume = Mathf.Clamp(
            walkSource.volume + (Player.main.Movement.IsWalking ? walkingFadeConstant : -walkingFadeConstant) * Time.deltaTime * AudioSystem.volume * currentWalkLevel,
            0,
            AudioSystem.volume * currentWalkLevel
        );

        if(footsteps && walkSource.volume / AudioSystem.volume <= 0.1f){
            walkSource.Stop(); 
            footsteps = false;
        }
        else if(!footsteps && walkSource.volume / AudioSystem.volume >= 0.1f){ 
            walkSource.Play(); 
            footsteps = true;
        }
    }

    public void TriggerJumpSFX(){
        AudioSource.PlayClipAtPoint(jumpClip, transform.position, AudioSystem.volume * jumpAudioLevel);
    }

    public void WindupChargeStart(){
        chargeSource.clip = chargeClip; 
        chargeSource.volume = AudioSystem.volume * chargeAudioLevel;
        chargeSource.Play();
        charging = true;
    }
    public void ReleaseCharge(){
        StopCharge();
        chargeSource.clip = chargeReleaseClip; 
        chargeSource.volume = AudioSystem.volume * chargeReleaseAudioLevel;
        chargeSource.Play();
        Invoke(nameof(StopCharge), 2);
    }

    public void StopCharge(){
        if(chargeSource.isPlaying){
            chargeSource.Stop();
        }
        charging = false;
    }

    public bool Charging(){
        return charging;
    }

    public void Pulse(){
        AudioSource.PlayClipAtPoint(pulseClip, transform.position, AudioSystem.volume * pulseAudioLevel);
    }
    public void DeathDelayed(){
        Invoke(nameof(Death), 0.5f);
    }
    public void Death(){
        AudioSource.PlayClipAtPoint(deathClip, transform.position, AudioSystem.volume * deathAudioLevel);
    }
    public void Push(){
        AudioSource.PlayClipAtPoint(pushClip, transform.position, AudioSystem.volume * pushAudioLevel);
    }
}
