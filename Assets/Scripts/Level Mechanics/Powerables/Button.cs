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

    [SerializeField] private bool isInteractable = true;
    private bool powerable1 = false;
    private bool powerable2 = false;

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
        if (isInteractable)
        {
            if (interactableSignifier.activeInHierarchy != InRange)
            { interactableSignifier.SetActive(InRange); }

            if (Animating)
            {
                _animTimer -= Time.deltaTime;
                DoAnimation();
            }

            if (Buffering) { _bufferTimer -= Time.deltaTime; }
            else { CheckStatusChange(); }
        }
        else
        {
            if (powerable1 && powerable2) //needed for puzzles where you need to activate multiple other things before a button is useable
            {
                isInteractable = true;
            }
        }
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

    public void TogglePow1(int num)
    {
        if (num == 1)
            powerable1 = !powerable1;
        else if (num == 2)
            powerable2 = !powerable2;
    }
}
