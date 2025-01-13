using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public static float layerDepth {get; private set;} = 1;
    [SerializeField] private float layerMod;

    //layer Depth * layer Mod = the rate at which 
    
    [SerializeField] private int layer;
    [SerializeField] private Vector2 startPos, startCamOffset;
    public Transform cam;


    // Start is called before the first frame update
    void Awake(){
        startPos = transform.position;
        startCamOffset = startPos - (Vector2)cam.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
