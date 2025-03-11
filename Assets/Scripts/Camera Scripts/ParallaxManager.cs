using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public static float layerDepth {get; private set;} = 1;
    [SerializeField] private float parallaxModifier;

    [SerializeField] private Transform referenceTf; 
    [SerializeField] private bool offsetFromReference { get {return referenceTf != null;}} 

    //layer Depth * layer Mod = the rate at which 
    
    [SerializeField] private int layer;
    [SerializeField] private Vector2 startPos, camOffset;
    public Transform cam;

    [SerializeField] private bool x, y;

    // Start is called before the first frame update
    void Awake(){
        startPos = transform.position;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update(){
        camOffset = startPos - (Vector2)cam.position;
        transform.position = new Vector2(
            x ? startPos.x - (camOffset.x * parallaxModifier) : startPos.x, 
            y ? startPos.y - (camOffset.y * parallaxModifier) : startPos.y);
    }
}
