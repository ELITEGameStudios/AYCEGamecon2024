using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public float fadeTime = 0.5f;
    public float liveFade;
    [SerializeField] private float activeFloat = 0f;
    [SerializeField] public MenuState menuState;
    [SerializeField] private Graphic[] graphics;
    [SerializeField] private Color[] graphicColors;

    public bool targetActive;
    public bool Active => menuState == MenuState.ACTIVE;
    public bool Switching => menuState == MenuState.SWITCHING;
    public bool Inactive => menuState == MenuState.INACTIVE;

    public enum MenuState{
        ACTIVE,
        SWITCHING,
        INACTIVE
    }

    public void Initialize(){
        graphicColors = new Color[graphics.Length];
        for (int i = 0; i < graphics.Length; i++){
            graphicColors[i] = graphics[i].color;
        }

        if(!Active){
            activeFloat = 0;
            foreach (Graphic item in graphics) { item.color = Color.clear; }
        }
    }
    
    void Awake(){
        activeFloat = Active ? 1 : 0;
        liveFade = fadeTime;
    }

    public void SwitchActive(bool target, float customFadeOut = -1){
        if(customFadeOut >= 0){liveFade = customFadeOut;}
        targetActive = target;
        menuState = MenuState.SWITCHING;
    }

    void Update()
    {
        if(menuState == MenuState.SWITCHING){
            activeFloat += 1/liveFade * Time.unscaledDeltaTime * (targetActive ? 1 : -1);
            for (int i = 0; i < graphics.Length; i++) { graphics[i].color = Color.Lerp(Color.clear, graphicColors[i], activeFloat); }
            
            if(activeFloat <= 0f && !targetActive){ 
                menuState = MenuState.INACTIVE; 
                activeFloat = 0f;
                liveFade = fadeTime;
            }
            else if(activeFloat >= 1.0f && targetActive){ 
                menuState = MenuState.ACTIVE; 
                activeFloat = 1f;
                liveFade = fadeTime;
            }
        }
    }
}
