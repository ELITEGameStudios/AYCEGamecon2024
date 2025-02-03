using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    
    public int bpm;
    public int beatsPerBar;
    public int barsPerMeasure;
    public int activeTracks;
    public int beatsElapsed, barsElapsed, measuresElapsed;
    public float timePerBeat, timePerBar, timePerMeasure;
    public float timer, timeInBar, timeInMeasure;

    [SerializeField] private AudioClip startupClip;
    List<AudioSource> sources;
    [SerializeField] private Sample currentSample;

    
    public static AudioSystem Instance {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null){ Instance = this;}
        else if(Instance != this){ Destroy(this);}
        
        sources ??= new List<AudioSource>();
        sources.Add(gameObject.AddComponent<AudioSource>());
        activeTracks = 1;
    }

    void Start(){
        SetVariablesViaBPM(bpm);
        if(startupClip != null){
            sources[0].clip = startupClip;
            sources[0].Play();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        activeTracks = sources.Count;

        timer += Time.fixedDeltaTime * sources[0].pitch;
        timeInBar = timer % timePerBar;
        timeInMeasure = timer % timePerMeasure;

        if(beatsElapsed != (int)(timer/timePerBeat)){
            beatsElapsed = (int)(timer/timePerBeat);
            BeatUpdate();

            if(barsElapsed != (int)(beatsElapsed/beatsPerBar)){
                barsElapsed = (int)(beatsElapsed/beatsPerBar);
                BarUpdate();

                if(measuresElapsed != (int)(barsElapsed/barsPerMeasure)){
                    measuresElapsed = (int)(barsElapsed/barsPerMeasure);
                    MeasureUpdate();
                }

            }

        }

    }

    void QueueNewSample(Sample sample){
        

    }

    void BeatUpdate(){
        // Debug.Log("beat");
    }
    void BarUpdate(){
        // Debug.Log("bar");
        // if(){// A sample is in queue

        // }
    }
    void MeasureUpdate(){
        // Debug.Log("measure");
    }

    void SetVariablesViaBPM(int bpm, int beatsPerBar = 4, int barsPerMeasure = 4){
        this.bpm = bpm;
        this.beatsPerBar = beatsPerBar;
        this.barsPerMeasure = barsPerMeasure;
        timePerBeat = 60f / bpm;
        timePerBar = timePerBeat * beatsPerBar;
        timePerMeasure = timePerBar * barsPerMeasure;
    }
}

[Serializable]
public struct Sample {
    public int bpm;
    public int beatsPerBar;
    public int barsPerMeasure;
    public int totalMeasures; 
    public AudioClip mainClip; // The required main clip your sample is based on
    public AudioClip transitionClip; // If you want a clip to serve as a transition to the main clip on the same track
    public int transitionClipBars;
    public bool naturalTransition; // For if the end of the main clip acts as the transition
    public bool hasTransitionClip {get {return transitionClip != null;}}
    public bool hasTransition {get {return hasTransition || naturalTransition;}}

}