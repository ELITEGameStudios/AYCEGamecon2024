using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] private Light2D fireLight, warnLight;
    [SerializeField] private GameObject warningPart;
    [SerializeField] private GameObject[] expParticles;
    [SerializeField] private float maxBrightness = 0.8f, warnBrightness = 11f, expTime, timer, warnTime;
    [SerializeField] private bool isActive;
    [SerializeField] private bool isWarning;
    // Start is called before the first frame update
    void Awake(){
        // expTime = BossFightManager.Instance.ExplosionTime * 3;
        expTime = 5;
    }

    public void WarnExplosion(float time){
        warnTime = time;
        timer = warnTime;
        isWarning = true;

        warningPart.SetActive(true);
        warnLight.enabled = true;
        foreach (GameObject particle in expParticles) { particle.SetActive(false); }
    }

    public void StartExplosion(){
        timer = expTime;
        isActive = true;
        isWarning = false;
        warningPart.SetActive(false);
        foreach (GameObject particle in expParticles) { particle.SetActive(true); }
    }

    public void StopExplosion(){
        timer = 0;
        isActive = false;

        warnLight.enabled = false;
        warningPart.SetActive(false);
        foreach (GameObject particle in expParticles) { particle.SetActive(false); }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0){
            timer -= Time.deltaTime;
            if(isWarning){
                warnLight.intensity =  (warnTime - timer) / warnTime * warnBrightness;
                return;
            }
            if(isActive){
                fireLight.intensity = timer / expTime * maxBrightness;
                warnLight.intensity = timer / expTime * warnBrightness;
                return;
            }

            fireLight.intensity = 0;
            warnLight.intensity = 0;
            warningPart.SetActive(false);
        }
        else{
            if(isActive){
                StopExplosion();
            }
        }
    }
}
