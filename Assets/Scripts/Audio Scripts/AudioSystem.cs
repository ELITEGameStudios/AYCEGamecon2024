using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class AudioSystem : MonoBehaviour
{
    
    public int bpm;
    public int beatsPerBar;
    public int barsPerMeasure;
    public int activeTracks;
    public int beatsElapsed, barsElapsed, measuresElapsed;
    public float timePerBeat, timePerBar, timePerMeasure;
    public float timer, timeInBar, timeInMeasure;

    [SerializeField] private int mainSourceIndex;
    [SerializeField] private AudioClip startupClip;
    [SerializeField] private Sample startupSample;
    List<AudioSource> sources;
    [SerializeField] private Sample currentSample;
    [SerializeField] private Sample queuedSample;


    public static float volume = 0.5f;
    [SerializeField] private Slider volumeSlider;

    public enum TransitionOnMarker{
        MEASURE,
        BAR,
        BEAT,
        IMMEDIATE,
        END,
        NONE
    };

    public enum TransitionType{
        SEGMENTED,
        CROSSFADE,
        SEAMLESS
    };

    [SerializeField] private TransitionOnMarker transitionMarker;
    [SerializeField] private TransitionType transitionType;
    
    public bool inTransition; // will become serialized private eventually
    public static AudioSystem Instance {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null){ Instance = this;}
        else if(Instance != this){ Destroy(this);}
        DontDestroyOnLoad(gameObject);
        
        sources ??= new List<AudioSource>();
        sources.Add(gameObject.AddComponent<AudioSource>());
        sources.Add(gameObject.AddComponent<AudioSource>());
        activeTracks = 1;
    }

    void Start(){
        
        QueueNewSample(startupSample, TransitionOnMarker.IMMEDIATE, TransitionType.SEAMLESS);
        // SetVariablesViaBPM(startupSample.bpm);
        
        SetVariablesViaBPM(bpm);
        if(startupClip != null){
            sources[0].clip = startupClip;
            sources[0].Play();
        }
    }
    void Update(){
        volume = volumeSlider.value;
    }

    void FixedUpdate()
    {
        activeTracks = sources.Count;

        // timer += Time.fixedDeltaTime * sources[0].pitch;
        timer += Time.fixedUnscaledDeltaTime;
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

    void QueueNewSample(Sample sample, TransitionOnMarker transitionOn, TransitionType type, float fadeIn = -1, float fadeOut = -1){
        if(transitionOn == TransitionOnMarker.NONE){return;} // NONE is not meant for creating transitions, it is meant to notate that nothing is queued. This is invalid.

        queuedSample = startupSample;

        if(fadeIn != -1){sample.fadeInSeconds = fadeIn;}
        if(fadeOut != -1){sample.fadeOutSeconds = fadeOut;}
        
        transitionMarker = transitionOn;
        transitionType = type;

        if(transitionOn == TransitionOnMarker.IMMEDIATE && !inTransition){
            StartCoroutine(TransitionAudio());
        }
    }

    void BeatUpdate(){
        if(transitionMarker == TransitionOnMarker.BEAT && !inTransition){
            StartCoroutine(TransitionAudio());
        }
    }

    void BarUpdate(){
        if(transitionMarker == TransitionOnMarker.BAR && !inTransition){
            StartCoroutine(TransitionAudio());
        }
    }

    void MeasureUpdate(){
        if(transitionMarker == TransitionOnMarker.MEASURE && !inTransition){
            StartCoroutine(TransitionAudio());
        }
    }

    void SetVariablesViaBPM(int bpm, int beatsPerBar = 4, int barsPerMeasure = 4){
        this.bpm = bpm;
        this.beatsPerBar = beatsPerBar;
        this.barsPerMeasure = barsPerMeasure;
        timePerBeat = 60f / bpm;
        timePerBar = timePerBeat * beatsPerBar;
        timePerMeasure = timePerBar * barsPerMeasure;
    }

    IEnumerator TransitionAudio(){
        inTransition = true;

        if(transitionType != TransitionType.SEGMENTED){
            bpm = queuedSample.bpm;
            SetVariablesViaBPM(bpm, queuedSample.beatsPerBar, queuedSample.barsPerMeasure);
        }


        AudioSource oldSource = sources[mainSourceIndex];
        AudioSource newSource;
        
        float oldVolume = oldSource.volume;
        float newVolume = volume * queuedSample.volumeConstant;

        float newClipfadeIn = queuedSample.fadeInSeconds;
        float oldClipFadeOut = currentSample.fadeOutSeconds;
        
        if(transitionType != TransitionType.SEAMLESS){
            mainSourceIndex = mainSourceIndex == 0 ? 1 : 0;
            newSource = sources[mainSourceIndex];

            newClipfadeIn = queuedSample.fadeInSeconds;
            oldClipFadeOut = currentSample.fadeOutSeconds;
        }
        else{
            newSource = oldSource;
            newSource.clip = queuedSample.mainClip;
        }


        float timer;

        switch(transitionType){
            case TransitionType.SEAMLESS:
                // Just immediately starts the new sample
                newSource.volume = newVolume;
                newSource.Play();
                oldSource.Stop();
                break;

            case TransitionType.CROSSFADE:
                // Setting the crossfade time to the largest fade time from both samples
                float resultantFadeTime = queuedSample.fadeInSeconds > currentSample.fadeOutSeconds ? 
                    queuedSample.fadeInSeconds : 
                    currentSample.fadeOutSeconds;
                timer = 0;
                
                newSource.Play();
                while (timer < resultantFadeTime){
                    // Fades the volumes at the same time
                    oldSource.volume = oldVolume - (oldVolume * timer/currentSample.fadeOutSeconds );
                    newSource.volume = newVolume * Mathf.Clamp(timer/queuedSample.fadeInSeconds, 0f, newVolume);
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                oldSource.Stop();

                newSource.volume = newVolume;
                
                break;
            case TransitionType.SEGMENTED:
                // Stops one and starts another with fades
                
                float fadeTime = currentSample.fadeOutSeconds;
                timer = fadeTime;
                
                while (timer > 0){
                    // Fades out old sample
                    oldSource.volume = oldVolume * timer/fadeTime;
                    timer -= Time.unscaledDeltaTime;
                    yield return null;
                }

                // Stops old volume
                timer = 0;
                oldSource.volume = 0;
                oldSource.Stop();
                
                // Setup audio system for new volume
                fadeTime = queuedSample.fadeInSeconds;
                bpm = queuedSample.bpm;
                SetVariablesViaBPM(bpm, queuedSample.beatsPerBar, queuedSample.barsPerMeasure);
                
                newSource.Play();
                
                while (timer > 0){
                    // Fades in new sample
                    newSource.volume = newVolume * timer/fadeTime;
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                newSource.volume = newVolume;
                break;
        }

        currentSample = queuedSample;
        transitionMarker = TransitionOnMarker.NONE;
        inTransition = false;
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

    public float volumeConstant; // Can only be between 0 and 1, is a multiplier to the volume set by the system itself to add an extra "mixer" systme to each sample


    // For fading, set upon queue
    public float fadeInSeconds;
    public float fadeOutSeconds;
}

