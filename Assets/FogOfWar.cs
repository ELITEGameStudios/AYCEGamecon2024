using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject FOWObject;
    [SerializeField] private bool hasPlayer;
    [SerializeField] private float opacity, fadeRate, minOpacity;
    [SerializeField] private Transform fowOffsetPoint;
    [SerializeField] private bool X, Y;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hasPlayer){
            if(opacity > 0){
                tilemap.color = Color.Lerp(Color.clear, Color.white, opacity);
                opacity -= Time.deltaTime * fadeRate;
            }
            else{
                tilemap.color = minOpacity > 0 
                    ? Color.Lerp(Color.clear, Color.white, opacity) 
                    : Color.clear;
            }

            UpdateFOWObject();
        }
        else if(opacity < 1){
            tilemap.color = Color.Lerp(Color.clear, Color.white, opacity);
            opacity += Time.deltaTime * fadeRate;
        }
        else{
            tilemap.color = Color.white;
        }

    }
    void UpdateFOWObject(){

    }
    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){
            hasPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(col == Player.main.MainCol){
            hasPlayer = false;
        }
    }

}
