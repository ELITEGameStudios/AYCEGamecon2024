using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioSource abilitiesSource;
    public static PlayerAudioManager instance {get; private set;}

    [SerializeField] private AudioClip pulseClip;

    bool footsteps;

    void Awake() {
        if(instance == null)instance = this;
        else if(instance != this) Destroy(gameObject);
    }

    void Start() {
        mainSource = Player.main.Audio;
    }

    void Update() {
        
    }

    public void Pulse(){
        abilitiesSource.clip = pulseClip;
        abilitiesSource.Play();
    }
}
