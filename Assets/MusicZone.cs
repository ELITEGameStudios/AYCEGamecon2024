using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [SerializeField] bool triggeredOnce = true, triggered;
    [SerializeField] Sample sampleToQueue;
    [SerializeField] AudioSystem.TransitionOnMarker marker;
    [SerializeField] AudioSystem.TransitionType transitionType;
    // [SerializeField] float fadeIn = -1, fadeOut = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col == Player.main.MainCol){
            if(triggered && triggeredOnce){return;}
            else{
                triggered = true;
                AudioSystem.Instance.QueueNewSample(sampleToQueue != null ? sampleToQueue : Sample.NothingSample, marker, transitionType);
            }
        }
    }
}
