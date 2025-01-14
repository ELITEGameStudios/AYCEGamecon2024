using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPulse : MonoBehaviour
{
    [SerializeField] private float radius, chargeTime, chargeTimer;
    public float ChargeTime {get {return chargeTime;} }
    public float ChargeTimer {get {return chargeTimer;} }
    public bool Ready {get{return chargeTimer < 0;}}

    void Pulse(){
        chargeTimer = chargeTime;
        PowerableObject[] powerables = FindObjectsByType<PowerableObject>(FindObjectsSortMode.None);
        foreach (PowerableObject powerable in powerables)
        {
            if(Vector2.Distance(powerable.transform.position, transform.position) <= radius ){
                powerable.Power(this);
            }
        }
    }

    void Update(){
        if(!Ready){ chargeTimer -= Time.deltaTime; }
        else if(Input.GetKeyDown(KeyCode.E)){
            Pulse();
        }
    }
}
