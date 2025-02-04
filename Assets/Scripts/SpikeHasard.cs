using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeHasard : MonoBehaviour
{
    [SerializeField] private float timer, warningTime, shakeIntensity;
    [SerializeField] private int shakeFrequency;
    [SerializeField] private SpikeHasardObject obj;

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){
            StartCoroutine(HasardCoroutine());
        }
    }

    IEnumerator HasardCoroutine(){
        Vector2 hasardObjectPos = obj.transform.position;
        while (timer > 0){
            obj.transform.position = 
                Vector2.Lerp( 
                    hasardObjectPos + Vector2.right * shakeIntensity, 
                    hasardObjectPos + Vector2.left * shakeIntensity,
                    Mathf.Sin(2*Mathf.PI * shakeFrequency * timer)
                );
            timer -= Time.deltaTime;
            yield return null;
        }
        obj.transform.position = hasardObjectPos;
        obj.Activate();
    }
}