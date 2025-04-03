using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TutorialUI[] startingTutorialUI;
    [SerializeField] private int targetFPS;
    public static GameManager Instance {get; private set;}

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) {Instance = this;}
        else if(Instance != this) {Destroy(this);}

        DontDestroyOnLoad(gameObject);
        SceneSystem.AddDontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Application.targetFrameRate = targetFPS;
    }
    public void EnableStartingTutorialUI(){

        if(startingTutorialUI != null){
            foreach(TutorialUI ui in startingTutorialUI) ui.enabled = true;
        }

    }

    public void SetMainPlayerFootsteps(bool toLab){
        if (toLab){
            PlayerAudioManager.instance.SetLabFootsteps();
        }
        else{
            PlayerAudioManager.instance.SetCaveFootsteps();
        }
    }
}
