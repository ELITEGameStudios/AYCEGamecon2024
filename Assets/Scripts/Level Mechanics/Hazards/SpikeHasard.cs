using System.Collections;
using UnityEngine;

public class SpikeHasard : MonoBehaviour
{
    [SerializeField] private float timer, warningTime, shakeIntensity, resetTime;
    [SerializeField] private int shakeFrequency;
    [SerializeField] private SpikeHasardObject obj;
    [SerializeField] private bool activated, colliderActivated;
    [SerializeField] private bool Resets {get {return resetTime > 0.0f;}}

    [SerializeField] private Quaternion initRot;

    void Awake(){
        timer = warningTime;
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject == Player.main.gameObject && colliderActivated && !activated){
            StartCoroutine(HasardCoroutine());
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol && !activated){
            StartCoroutine(HasardCoroutine());
        }
    }

    IEnumerator HasardCoroutine(){
        Vector2 hasardObjectPos = obj.transform.position;
        initRot = obj.transform.rotation;

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
        activated = true;

        if(Resets){
            yield return new WaitForSeconds(resetTime);
            
            obj.Deactivate();
            obj.transform.position = hasardObjectPos;
            obj.transform.rotation = initRot;
            
            activated = false;
            timer = warningTime;
        }
    }
}