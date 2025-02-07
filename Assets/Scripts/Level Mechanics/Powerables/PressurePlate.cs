using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : PowerableObject
{
    public List<Collision2D> collisionList;

    private Vector2 inactivePos, activePos, startPos, targetPos;

    [SerializeField] private float _timer, _bufferTime, plateSteepness;
    bool Buffering {get{return _timer > 0;}}
    bool HasWeight {get{return collisionList.Count > 0;}}

    protected static string[] tags = new string[]{
        "Player",
        "PhysObject"
    };

    // Start is called before the first frame update
    void Awake(){
        BaseSetup();
        collisionList = new List<Collision2D>();
        isSwitch = true; // Must be a switch to function correctly
        unaffectedByPulse = true; 

        inactivePos = transform.position;
        activePos = transform.position + (Vector3.down * plateSteepness);
    }

    // Update is called once per frame
    void Update()
    {
        if(Buffering){ 
            _timer -= Time.deltaTime; 
            DoAnimation();
        }
        else{CheckStatusChange();}
    }

    void CheckStatusChange(){
        if(HasWeight != active){
            Power(HasWeight);
            _timer = _bufferTime;
            SetAnimation();
        }
    }

    void SetAnimation(){
        startPos = transform.position; 
        targetPos = active ? activePos : inactivePos;
    }

    void DoAnimation(){
        transform.position = Vector2.Lerp(targetPos, startPos, _timer / _bufferTime);
    }


    void OnCollisionEnter2D(Collision2D col){
        if(IsValidObject(col.gameObject.tag)){
            collisionList.Add(col);
        }
    }
    void OnCollisionExit2D(Collision2D col){
        if(IsValidObject(col.gameObject.tag)){
            collisionList.Remove(col); 
        }
    }

    bool IsValidObject(string input){
        foreach (string tag in tags){
            if(input == tag){ return true; }
        }

        return false; 
    }
}
