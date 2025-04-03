using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeIn, fadeOut, hold;
    // public bool fading {get; private set;} = false;
    public bool fading = false;
    public static FadeScreen Instance {get; private set;}


    void Awake(){  
        if(Instance == null){Instance = this;}
        else if(Instance != this){Destroy(gameObject);}
    }

    void Update(){
        if(!fading && image.color != Color.clear){
            image.color = Color.clear;
        }
    }

    public void FadeInOut(float fadeIn, float hold, float fadeOut){
        if(fading){return;}

        this.fadeIn = fadeIn;
        this.fadeOut = fadeOut; 
        this.hold = hold;
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine(){
        fading = true;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // Fade in
        float timer = 0;
        while (timer < fadeIn){
            image.color = Color.Lerp(Color.clear, Color.black, curve.Evaluate(timer/fadeIn));
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Hold
        image.color = Color.black;
        yield return new WaitForSecondsRealtime(hold);
        
        // Fade out
        timer = fadeOut;
        while (timer > 0){
            image.color = Color.Lerp(Color.clear, Color.black, curve.Evaluate(timer/fadeOut));
            timer -= Time.unscaledDeltaTime;
            yield return null;
        }
        image.color = Color.clear;

        fading = false;
    }
}
