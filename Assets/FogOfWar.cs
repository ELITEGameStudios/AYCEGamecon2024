using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private Transform FOWObject;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private float minDistance;


    // Start is called before the first frame update
    void Awake()
    {
        startScale = FOWObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("FOWPoint");
        
        if(objs.Length > 0){

            float closestDist = Mathf.Infinity;
            GameObject closestObj = objs[0];

            foreach (GameObject obj in objs){
                float distance = Vector2.Distance(FOWObject.transform.position, transform.position);
                if(distance < closestDist){
                    closestObj = obj;
                    closestDist = distance;
                }
            }

            FOWObject.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.2f, closestDist / minDistance);


        }
    }
    void UpdateFOWObject(){

    }
}
