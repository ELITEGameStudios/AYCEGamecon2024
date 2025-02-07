using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : PowerableObject
{

    public List<Collision2D> collisionList;

    private Vector2 inactivePos, activePos, startPos, targetPos;
    [SerializeField] private GameObject interactableSignifier;
    [SerializeField] private float _bufferTimer, _bufferTime, _animTimer, _animTime, buttonSteepness, interactableRange;
    [SerializeField] private bool left;
    bool Buffering {get{return _bufferTimer > 0;}}
    bool Animating {get{return _animTimer > 0;}}
    bool InRange {get{return Vector2.Distance(Player.main.transform.position, transform.position) <= interactableRange;}}



    // Start is called before the first frame update
    void Awake(){
        BaseSetup();
        collisionList = new List<Collision2D>();
        isSwitch = true; // Must be a switch to function correctly
        unaffectedByPulse = true; 

        inactivePos = transform.position;
        activePos = transform.position + ((left ? Vector3.right : Vector3.left) * buttonSteepness);
    }

    // Update is called once per frame
    void Update()
    {
        if(interactableSignifier.activeInHierarchy != InRange)
        {interactableSignifier.SetActive(InRange);}

        if(Animating){ 
            _animTimer -= Time.deltaTime; 
            DoAnimation();
        }

        if(Buffering){ _bufferTimer -= Time.deltaTime;  }
        else{CheckStatusChange();}
    }

    void CheckStatusChange(){
        if( (!active && InRange && InputManager.interact.pressedThisFrame) || active )
        { Toggle(); }
    }

    void Toggle(){
        Power(!active);
        if(active){ _bufferTimer = _bufferTime; };
        SetAnimation();
    }

    void SetAnimation(){
        startPos = transform.position; 
        targetPos = active ? activePos : inactivePos;
        _animTimer = _animTime;
    }

    void DoAnimation(){
        transform.position = Vector2.Lerp(targetPos, startPos, _animTimer / _animTime);
    }
}
