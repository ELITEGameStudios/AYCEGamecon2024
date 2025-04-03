using System.Collections;
using System.Collections.Generic;
using SoundSystems;
using UnityEngine;

public class Switch : PowerableObject
{
    [SerializeField] private float _timer, _bufferTime, interactableRange, switchAngle;
    [SerializeField] private GameObject interactableSignifier;
    [SerializeField] private Quaternion activeRot;
    [SerializeField] private Quaternion inactiveRot;
    [SerializeField] private Quaternion startRot, targetRot;
    [SerializeField] private bool left;
    [SerializeField] private bool state = false;
    [SerializeField] private bool animate = true, flipOnce, flippedOnce;

    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator lightAnimator;

    bool Buffering {get{return _timer > 0;}}
    bool InRange {get{return Vector2.Distance(Player.main.transform.position, transform.position) <= interactableRange;}}

    void Awake(){
        BaseSetup();
        
        if(!nameOfSound.Contains("AL")) {isSwitch = true;} // Must be a switch to function correctly, unless audio log is integrated within this script
        unaffectedByPulse = true; 
        
        if(left){
            activeRot = Quaternion.Euler(Vector3.back * (180 + switchAngle));
            inactiveRot= Quaternion.Euler(Vector3.back * (180 - switchAngle));
        }
        else{
            activeRot = Quaternion.Euler(Vector3.forward * switchAngle);
            inactiveRot= Quaternion.Euler(Vector3.forward * -switchAngle);
        }

        transform.rotation = inactiveRot;
        if(interactableSignifier != null) interactableSignifier.SetActive(false); //discontinuing this until further notice
    }
    // Start is called before the first frame update
    void Update()
    {
        // if(interactableSignifier.activeInHierarchy != InRange)

        if(Buffering){ 
            _timer -= Time.deltaTime;
            DoAnimation();
        }
        else {CheckStatusChange();}

        leverAnimator.SetBool("Interact1", activeRot == targetRot);
        lightAnimator.SetBool("Interact1", activeRot == targetRot);
    }

    void CheckStatusChange(){
        // if(InRange && InputManager.interact.pressedThisFrame){
        //     Power(!active);
        // }
    }

    public override void Power(bool active)
    {
        if(flippedOnce && flipOnce){return;}

        base.Power(active);
        _timer = _bufferTime;
        SetAnimation();
        
        if(playsSound){
            Debug.Log("Probably an audio log");
            EnvironmentalSoundSystem.instance.CreateEnvSound(nameOfSound, soundToPlay, gameObject);
        }
        flippedOnce = true;
    }

    void SetAnimation(){
        if(animate){
            startRot = transform.rotation; 
            targetRot = active ? activeRot : inactiveRot;
        }
    }

    void DoAnimation(){
        transform.rotation = Quaternion.Lerp(targetRot, startRot, _timer / _bufferTime);
    }
}
