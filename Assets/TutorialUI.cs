using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] float waitTime = 5f, fadeTime = 1, interval, postWaitTime = 2.5f;
    [SerializeField] bool active, inZone, appeared, disableImmediately;
    [SerializeField] Graphic[] graphics;
    [SerializeField] Color[] graphicColors;
    // Start is called before the first frame update
    void Awake()
    {
        graphicColors ??= new Color[graphics.Length];
        for (int i = 0; i < graphics.Length; i++){
            graphicColors[i] = graphics[i].color;
            graphics[i].color = Color.clear;
        }

        if(disableImmediately){this.enabled = false;}
    }

    // Update is called once per frame
    void Update()
    {
        if(inZone){
            if(waitTime > 0){
                waitTime -= Time.deltaTime;
            }
            else if(!active && !appeared){
            active = true;
                StartCoroutine(Appear());
            }

        }
        else if(appeared && active){
            active = false;
            StartCoroutine(Dissapear());
        }
    }

    IEnumerator Appear(){
        float timer = fadeTime;
        
        // Without Interval
        if(interval <= 0){
            while(timer > 0){
                for (int i = 0; i < graphics.Length; i++){
                    
                    graphics[i].color = Color.Lerp(graphicColors[i], Color.clear, timer/fadeTime);
                    
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }
        else{
            // With Interval
            while(timer + interval * (graphics.Length - 1) > 0){
                for (int i = 0; i < graphics.Length; i++){
                    
                    graphics[i].color = Color.Lerp(graphicColors[i], Color.clear, timer + interval*i /fadeTime);
                    
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }

        // Final Setup
        for (int i = 0; i < graphics.Length; i++){
            graphics[i].color = graphicColors[i];
        }
        appeared = true;
    }

    IEnumerator Dissapear(){
        float timer = fadeTime;
        
        // Without Interval
        if(interval <= 0){
            while(timer > 0){
                for (int i = 0; i < graphics.Length; i++){
                    
                    graphics[i].color = Color.Lerp(Color.clear, graphicColors[i], timer/fadeTime);
                    
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }
        else{
            // With Interval
            while(timer + interval * graphics.Length - 1 > 0){
                for (int i = 0; i < graphics.Length; i++){
                    
                    graphics[i].color = Color.Lerp(Color.clear, graphicColors[i], timer + interval*i /fadeTime);
                    
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
        }

        // Final Setup
        for (int i = 0; i < graphics.Length; i++){
            graphics[i].color = Color.clear;
        }
        waitTime = postWaitTime;
        appeared = false;
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){inZone = true;}
    }
    void OnTriggerExit2D(Collider2D col){
        if(col == Player.main.MainCol){inZone = false;}
    }
}
