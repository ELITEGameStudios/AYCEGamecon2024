using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int targetFPS;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneSystem.AddDontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Application.targetFrameRate = targetFPS;
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
