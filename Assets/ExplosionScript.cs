using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] private Light2D fireLight;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private float maxBrightness = 1.5f, expTime, timer;
    [SerializeField] private bool isActive;
    // Start is called before the first frame update
    void Awake(){
        // expTime = BossFightManager.Instance.ExplosionTime * 3;
        expTime = 5;
    }

    public void StartExplosion(){
        timer = expTime;
        isActive = true;
    }

    public void StopExplosion(){
        timer = 0;
        isActive = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0){
            timer -= Time.deltaTime;
            fireLight.intensity = timer / expTime * maxBrightness;
        }
        else{
            if(isActive){
                StopExplosion();
            }
        }
    }
}
