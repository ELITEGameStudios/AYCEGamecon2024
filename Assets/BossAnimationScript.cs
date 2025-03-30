using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject animatedObjParent;
    [SerializeField] private GameObject staticObjParent;
    [SerializeField] private GameObject[] deathParts; // index 0 will be main body
    [SerializeField] private SpriteRenderer[] lights;
    [SerializeField] private Transform mainHead, explosionForceOrgin;
    [SerializeField] private SpriteRenderer backLeg;
    [SerializeField] private BossScript bossReference;
    [SerializeField] private float deathExplosionForce, explosionDistance, explosionForce;

    [Header("Dash and Normal Poses Dada")]
    [SerializeField] private Sprite normalLeg;
    [SerializeField] private Sprite dashedLeg;
    


    [Header("Head Tilt Debug")]
    [SerializeField] private Quaternion targetTiltAngle, startingTiltAngle;
    [SerializeField] private AnimationCurve activeCurve;
    public bool activated = false;
    [SerializeField] private float targetTiltTime, tiltTimer;
    
    
    [SerializeField] private Color baseColor;



    [Header("Lights Debug")]
    [SerializeField] private float targetColorTime; 
    [SerializeField] private float colorTimer, oscilator,
        targetFrequencyTime, frequencyTimer, targetFrequency, currentFrequency, startingFrequency;
    [SerializeField] private Color targetColor, startingColor;



    private bool Tilting {get {return tiltTimer < targetTiltTime && activated;}}
    private bool ChangingColor {get {return colorTimer < targetColorTime;}}
    private bool ChangingFrequency {get {return frequencyTimer < targetFrequencyTime;}}
    public float CurrentColor { get {return lights[0].color.a;}}
    public bool oscilating {get {return currentFrequency > 0;}} // a frequency value of less than 0 removes oscilation, lights are just set to whatever Color they need to be
    public bool MainModelActive {get {return staticObjParent.activeInHierarchy;}}
    public int flipStatus = 1;

    // Start is called before the first frame update
    void Start()
    {
        // SetModel(false);
    }

    // Update is called once per frame
    void Update(){
        // if(MainModelActive){
            oscilator += Time.deltaTime;
            
            // For Head Tilt
            if(Tilting){
                Debug.Log("Tilting");
                float proportionalTime = tiltTimer / targetTiltTime; 
                mainHead.rotation = Quaternion.SlerpUnclamped(startingTiltAngle, targetTiltAngle, activeCurve.Evaluate(proportionalTime));
                tiltTimer += Time.deltaTime;
            }
            else if(mainHead.rotation != targetTiltAngle){
                mainHead.rotation = targetTiltAngle;
            }

            // For Color change
            float proportionalColorTime = colorTimer / targetColorTime; 
            if(ChangingColor){
                Debug.Log("Color is changing");
                if(!oscilating){
                    Debug.Log("Unoscilated Color is changing");
                    foreach (SpriteRenderer item in lights){
                        item.color = Color.Lerp(startingColor, targetColor, proportionalColorTime);
                    }
                }
                colorTimer += Time.deltaTime;
            }
            // For Oscilation
            if(oscilating){
                Color oscColor = targetColor;
                if(ChangingColor){oscColor = Color.Lerp(baseColor, targetColor, proportionalColorTime);} // support for changing colors while oscilating
                // Debug.Log("Color is oscilating with frequeny " + currentFrequency);
                foreach (SpriteRenderer item in lights){
                    item.color = Color.Lerp(baseColor, oscColor, 0.5f * Mathf.Sin(2*Mathf.PI * (currentFrequency * oscilator - 0.25f)) + 0.5f);
                    Debug.Log("New color lerp at " + 0.5f * Mathf.Sin(2*Mathf.PI * (currentFrequency * oscilator - 0.25f)) + 0.5f);
                }
            }
            else if(lights[0].color != targetColor){
                foreach (SpriteRenderer item in lights){
                    Debug.Log("Fully set colors");
                    item.color = targetColor;
                }
            }

            // For Frequency change
            if(ChangingFrequency){
                Debug.Log("Changing frequency");
                float proportionalTime = frequencyTimer / targetFrequencyTime; 
                currentFrequency = Mathf.Lerp(startingFrequency, targetFrequency, proportionalTime);
                frequencyTimer += Time.deltaTime;
            }
            else if(currentFrequency != targetFrequency){
                currentFrequency = targetFrequency;
            }

        // }
    }

    public void SetModel(bool toReconstructedModel){
        staticObjParent.SetActive(!toReconstructedModel);
        animatedObjParent.SetActive(toReconstructedModel);

        if(!toReconstructedModel){
            targetTiltAngle = Quaternion.Euler(0, 0, 0);
            mainHead.rotation = targetTiltAngle;
        }
        else{
            animatedObjParent.transform.localScale = new Vector3(bossReference.flipDir, 1, 1);
            flipStatus = bossReference.flipDir;
        }
    }


    public void TiltHead(float angle,AnimationCurve curve, float time = 1){
        startingTiltAngle = mainHead.rotation;
        targetTiltAngle = Quaternion.Euler(0, 0, angle * flipStatus);
        targetTiltTime = time;
        activeCurve = curve;
        tiltTimer = 0;
        if(!activated){activated = true;}
    }

    public void SetColor(Color newColor, float timeToNewColor){
        startingColor = oscilating ? targetColor : lights[0].color;
        targetColor = newColor;
        targetColorTime = timeToNewColor;
        colorTimer = 0;
    }

    public void SetOscilation(float newOscilation, float time){
        startingFrequency = currentFrequency;
        targetFrequency = newOscilation;
        targetFrequencyTime = time;
        frequencyTimer = 0;
        oscilator = 0;
    }

    public void SetPose(bool dashingPose){
        backLeg.sprite = dashingPose ? dashedLeg : normalLeg;
    }

    public void ExplosionBoom(){
        
        foreach (GameObject chain in GameObject.FindGameObjectsWithTag("chain")){
            // part.transform.localScale = animatedObjParent.transform.localScale;
            // part.SetActive(true);
            // part.transform.SetParent(null);
            float distance = Vector2.Distance(explosionForceOrgin.position, chain.transform.position);
            if( distance > explosionDistance){continue;}

            Vector2 closestPoint = chain.GetComponent<Collider2D>().ClosestPoint(explosionForceOrgin.position);
            Vector2 forceVector = (closestPoint - (Vector2)explosionForceOrgin.position).normalized * explosionForce * Random.Range(0.1f, 1f) *(explosionDistance - distance) / explosionDistance;
            
            chain.GetComponent<Rigidbody2D>().AddForceAtPosition(forceVector, explosionForceOrgin.position);
        }
    }

    public void DeathAnimation(){
        deathParts[0].transform.rotation = mainHead.rotation;
        foreach (GameObject part in deathParts){
            part.transform.localScale = animatedObjParent.transform.localScale;
            part.SetActive(true);
            part.transform.SetParent(null);

            Vector2 closestPoint = part.GetComponent<Collider2D>().ClosestPoint(transform.position);
            Vector2 forceVector = (closestPoint - (Vector2)BossFightManager.Instance.transform.position).normalized * deathExplosionForce;
            part.GetComponent<Rigidbody2D>().AddForceAtPosition(forceVector, transform.position);
        }
    }
}
