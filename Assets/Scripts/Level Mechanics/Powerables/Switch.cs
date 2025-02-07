using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : PowerableObject
{
    [SerializeField] private float _timer, _bufferTime, interactableRange, switchAngle;
    [SerializeField] private GameObject interactableSignifier;
    [SerializeField] private Quaternion activeRot;
    [SerializeField] private Quaternion inactiveRot;
    [SerializeField] private Quaternion startRot, targetRot;
    [SerializeField] private bool left;
    bool Buffering {get{return _timer > 0;}}
    bool InRange {get{return Vector2.Distance(Player.main.transform.position, transform.position) <= interactableRange;}}

    void Awake(){
        BaseSetup();
        
        isSwitch = true; // Must be a switch to function correctly
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
    }
    // Start is called before the first frame update
    void Update()
    {
        if(interactableSignifier.activeInHierarchy != InRange)
        {interactableSignifier.SetActive(InRange);}

        if(Buffering){ 
            _timer -= Time.deltaTime; 
            DoAnimation();
        }
        else{CheckStatusChange();}
    }

    void CheckStatusChange(){
        if(InRange && InputManager.interact.pressedThisFrame){
            Power(!active);
            _timer = _bufferTime;
            SetAnimation();
        }
    }

    void SetAnimation(){
        startRot = transform.rotation; 
        targetRot = active ? activeRot : inactiveRot;
    }

    void DoAnimation(){
        transform.rotation = Quaternion.Lerp(targetRot, startRot, _timer / _bufferTime);
    }
}
