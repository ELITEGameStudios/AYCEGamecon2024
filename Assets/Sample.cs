using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sample", menuName ="Sample", order =2)]
public class Sample : ScriptableObject
{
    public int bpm;
    public int beatsPerBar;
    public int barsPerMeasure;
    public int totalMeasures; 
    public int totalBars; 
    public AudioClip mainClip; // The required main clip your sample is based on
    public AudioClip transitionClip; // If you want a clip to serve as a transition to the main clip on the same track
    public int transitionClipBars;
    public bool naturalTransition; // For if the end of the main clip acts as the transition
    public bool loop; // For if the end of the main clip acts as the transition
    public bool hasTransitionClip {get {return transitionClip != null;}}
    public bool hasTransition {get {return hasTransition || naturalTransition;}}
    public bool usesTotalBars {get {return totalBars > 0;}}

    public float volumeConstant; // Can only be between 0 and 1, is a multiplier to the volume set by the system itself to add an extra "mixer" systme to each sample


    // For fading, set upon queue
    public float fadeInSeconds;
    public float fadeOutSeconds;

    public Sample(AudioClip clip, int bpm, int beatsPerBar, int barsPerMeasure, int totalMeasures, float volumeConstant, AudioClip transitionClip = null, int transitionClipBars = -1, bool naturalTransition = true, float fadeIn = 2, float fadeOut = 2, int totalBars = 0, bool loop = false){
        this.mainClip = clip;
        this.bpm = bpm;
        this.beatsPerBar = beatsPerBar;
        this.barsPerMeasure = barsPerMeasure;
        this.totalMeasures = totalMeasures;
        this.transitionClip = transitionClip;
        this.transitionClipBars = transitionClipBars;
        this.naturalTransition = naturalTransition;
        this.volumeConstant = volumeConstant;
        this.loop = loop;
        this.totalBars =totalBars;
        fadeInSeconds = fadeIn;
        fadeOutSeconds = fadeOut;
    }

    public static readonly Sample NothingSample = new(null, 120, 1, 1, 1, 1);
}
