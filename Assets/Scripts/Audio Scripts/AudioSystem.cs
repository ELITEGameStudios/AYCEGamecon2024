using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
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
    public bool active;
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

        foreach (AudioSource item in sources) { item.playOnAwake = false; }
        activeTracks = 1;
    }

    void Start(){
        
        QueueNewSample(startupSample, TransitionOnMarker.IMMEDIATE, TransitionType.SEAMLESS);
        // SetVariablesViaBPM(startupSample.bpm);
        // SetVariablesViaBPM(bpm);
    }
    void Update(){
        volume = volumeSlider.value;
    }

    void FixedUpdate()
    {
        if(active){
            activeTracks = sources.Count;
            // if(!Application.isFocused) {return;}
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
    }

    void QueueNewSample(Sample sample, TransitionOnMarker transitionOn, TransitionType type, float fadeIn = -1, float fadeOut = -1){
        if(transitionOn == TransitionOnMarker.NONE){return;} // NONE is not meant for creating transitions, it is meant to notate that nothing is queued. This is invalid.

        queuedSample = sample;

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

        Debug.Log("Beat");
    }

    void BarUpdate(){
        if(transitionMarker == TransitionOnMarker.BAR && !inTransition){
            StartCoroutine(TransitionAudio());
        }
        Debug.Log("Bar");
    }

    void MeasureUpdate(){
        if(transitionMarker == TransitionOnMarker.MEASURE && !inTransition){
            StartCoroutine(TransitionAudio());
        }
        Debug.Log("Measure");

        if(measuresElapsed == currentSample.totalMeasures){
            if(!inTransition && transitionMarker == TransitionOnMarker.END){
                StartCoroutine(TransitionAudio());
            }
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

    void ResetTimers(){
        beatsElapsed = 0;
        barsElapsed = 0;
        measuresElapsed = 0;
    }

    IEnumerator TransitionAudio(){
        inTransition = true;
        if(!active){
            yield return new WaitForSeconds(1);
            ResetTimers();
            active = true;
        }
        Debug.Log("Started transition");

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
        }

        if(newSource.isPlaying){newSource.Stop();}
        newSource.clip = queuedSample.mainClip;
        float timer;

        ResetTimers();
        switch(transitionType){
            case TransitionType.SEAMLESS:
                // Just immediately starts the new sample
                newSource.volume = newVolume;
                if(newSource.clip != null){newSource.Play(); Debug.Log("Playing on new source");}
                if(newSource != oldSource){oldSource.Stop();}
                
                Debug.Log("Executed Seamless transition");
                break;

            case TransitionType.CROSSFADE:
                // Setting the crossfade time to the largest fade time from both samples
                float resultantFadeTime = queuedSample.fadeInSeconds > currentSample.fadeOutSeconds ? 
                    queuedSample.fadeInSeconds : 
                    currentSample.fadeOutSeconds;
                timer = 0;
                
                if(newSource.clip != null){newSource.Play();}
                while (timer < resultantFadeTime){
                    // Fades the volumes at the same time
                    oldSource.volume = oldVolume - (oldVolume * timer/currentSample.fadeOutSeconds );
                    newSource.volume = newVolume * Mathf.Clamp(timer/queuedSample.fadeInSeconds, 0f, newVolume);
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                oldSource.Stop();

                newSource.volume = newVolume;
                
                Debug.Log("Executed Crossfade transition");
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
                
                if(newSource.clip != null){newSource.Play();}
                
                while (timer > 0){
                    // Fades in new sample
                    newSource.volume = newVolume * timer/fadeTime;
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                newSource.volume = newVolume;
                Debug.Log("Executed Segmented transition");
                break;
        }

        currentSample = queuedSample;
        transitionMarker = TransitionOnMarker.NONE;
        inTransition = false;
        QueueNewSample(Sample.NothingSample, TransitionOnMarker.END, TransitionType.SEGMENTED);
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

    public Sample(AudioClip clip, int bpm, int beatsPerBar, int barsPerMeasure, int totalMeasures, float volumeConstant, AudioClip transitionClip = null, int transitionClipBars = -1, bool naturalTransition = true, float fadeIn = 2, float fadeOut = 2){
        this.mainClip = clip;
        this.bpm = bpm;
        this.beatsPerBar = beatsPerBar;
        this.barsPerMeasure = barsPerMeasure;
        this.totalMeasures = totalMeasures;
        this.transitionClip = transitionClip;
        this.transitionClipBars = transitionClipBars;
        this.naturalTransition = naturalTransition;
        this.volumeConstant = volumeConstant;
        fadeInSeconds = fadeIn;
        fadeOutSeconds = fadeOut;
    }

    public static readonly Sample NothingSample = new(null, 120, 1, 1, 1, 1);
}

