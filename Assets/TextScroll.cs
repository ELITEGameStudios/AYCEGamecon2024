using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextScroll : MonoBehaviour
{
    [SerializeField] private TextEntry[] entries;
    [SerializeField] private Image bg;
    [SerializeField] private Color bgColor;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float timer, bgFadeTime, pauseSeconds = 2;

    // Update is called once per frame
    public void BeginSequence(){
        StartCoroutine(ScrollCoroutine());
    }

    IEnumerator ScrollCoroutine(){
        while (timer < bgFadeTime){
            bg.color = Color.Lerp(Color.clear, bgColor, timer / bgFadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        bg.color = bgColor;

        yield return new WaitForSeconds(pauseSeconds);

        foreach (TextEntry entry in entries){
            
            text.text = entry.phrase;
            timer = 0;
            while (timer < entry.fade){
                text.color = Color.Lerp(Color.clear, entry.color, timer / entry.fade);
                timer += Time.deltaTime;
                yield return null;
            }
            
            text.color = entry.color;
            yield return new WaitForSeconds(entry.hold);
            
            while (timer > 0){
                text.color = Color.Lerp(Color.clear, entry.color, timer / entry.fade);
                timer -= Time.deltaTime;
                yield return null;
            }
            text.color = Color.clear;
        }
        
        Application.Quit();
    }
}


[Serializable]
public class TextEntry{


    public float fade;
    public float hold;
    public string phrase;
    public Color color;


}
