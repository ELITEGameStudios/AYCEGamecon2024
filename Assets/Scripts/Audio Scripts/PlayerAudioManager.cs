using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource abilitiesSource;
    // [SerializeField] private AudioSource jumpSource;
    [SerializeField] private AudioSource[] sourcesList;

    public static PlayerAudioManager instance {get; private set;}

    [SerializeField] private AudioClip pulseClip;
    [SerializeField] private AudioClip footstepsClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip pushClip;
    [SerializeField] private float walkingFadeConstant, walkAudiolevel = 1, pushAudioLevel = 1, pulseAudioLevel = 1, jumpAudioLevel = 1;

    bool footsteps;

    void Awake() {
        if(instance == null)instance = this;
        else if(instance != this) Destroy(gameObject);
        sourcesList = new[]{mainSource, abilitiesSource};
    }

    void Start() {
        mainSource = Player.main.Audio;
        walkSource.clip = footstepsClip;
        footsteps = false;
    }

    void Update() {
        foreach (AudioSource source in sourcesList) { source.volume = AudioSystem.volume; }

        walkSource.volume = Mathf.Clamp(
            walkSource.volume + (Player.main.Movement.IsWalking ? walkingFadeConstant : -walkingFadeConstant) * Time.deltaTime * AudioSystem.volume * walkAudiolevel,
            0,
            AudioSystem.volume * walkAudiolevel
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

    public void Pulse(){
        AudioSource.PlayClipAtPoint(pulseClip, transform.position, AudioSystem.volume * pulseAudioLevel);
    }
    public void Push(){
        AudioSource.PlayClipAtPoint(pushClip, transform.position, AudioSystem.volume * pushAudioLevel);
    }
}
