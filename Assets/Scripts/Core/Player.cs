using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerPulse pulse;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D mainCol;
    [SerializeField] private AudioSource audio;
    [SerializeField] private float deathVel;
    [SerializeField] private GameObject deathPart;
    [SerializeField] private PlayerAnimations playerAnimations;

    public PlayerMovement Movement { get { return movement; } }
    public PlayerPulse Pulse { get { return pulse; } }
    public Rigidbody2D Rb {get {return rb;}}
    public  Collider2D MainCol {get {return mainCol;}}
    public AudioSource Audio {get; private set;}
        
    public static Player main {get; private set;}
    public bool dead {get; private set;} = false; 
    public int powerLevel {get; private set;} = 0; // Just represents the amount of mechanics we unlocked, this isnt shown to the user
    public bool unlockedCharge {get {return powerLevel >= 1;}}
    public bool unlockedWallRun {get {return powerLevel >= 1;}}
    public bool unlockedPowerJump {get {return powerLevel >= 2;}}
    
    
    void Awake()
    {
        if(main == null) main = this;
        else if(main != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        SceneSystem.AddDontDestroyOnLoad(gameObject);
    }
    
    public void Die(bool immediate = false){
        if(dead){return;}

        pulse.enabled = false;
        movement.enabled = false;
        GetComponent<Light2D>().enabled = false;
        dead = true;
        

        Invoke(nameof(Respawn), 2f);

        if(immediate){
            Dissapear();
            FadeScreen.Instance.FadeInOut(0, 2, 1);
            PlayerAudioManager.instance.Death();
        }
        else{
            //Invoke(nameof(Dissapear), 0.5f);
            Dissapear();
            PlayerAudioManager.instance.DeathDelayed();
            
            // GetComponent<Rigidbody2D>().velocity = Vector2.up * deathVel;

            CameraFollowScript.Instance.Shake(4, 1, 1);
            FadeScreen.Instance.FadeInOut(1, 1, 1);
            
        }
    }

    public void Dissapear(){
        foreach (SpriteRenderer renderer in movement.SpriteList){
            renderer.enabled = false;
        }
        GameObject particles = Instantiate(deathPart, transform.position, transform.rotation);
        Destroy(particles, 1);
    }

    public void Appear(){
        foreach (SpriteRenderer renderer in movement.SpriteList){
            renderer.enabled = true;
        }
    }

    public IEnumerator ResetLevel(){
        UIManager.Instance.OpenMenuViaState(UIManager.MenuState.NONE, false);
        FadeScreen.Instance.FadeInOut(1, 1, 1);
        yield return new WaitForSecondsRealtime(1.5f);
        SetToScenePos();
    }

    public void SetToScenePos(){
        transform.position = SceneData.currentScene.transform.position;
        CameraFollowScript.Instance.SetToPlayer();
    }

    public void Respawn(){
        if(!dead){return;}
        RespawnSystem.Instance.RespawnPlayer();
        
        Appear();
        pulse.enabled = true;
        movement.enabled = true;
        GetComponent<Light2D>().enabled = true;
        dead = false;
    }

    public void SetUnlock(int newUnlock){
        main.powerLevel = newUnlock;
        if(newUnlock == 1){playerAnimations.ChangeAnimation("Charging");}
        // else if(newUnlock == 2){playerAnimations.ChangeAnimation("");}
    }
    
}
