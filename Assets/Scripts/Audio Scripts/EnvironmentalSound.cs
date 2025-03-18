using UnityEngine;

namespace SoundSystems{
    [System.Serializable]
    [CreateAssetMenu(fileName ="New Environmental Sound", menuName ="EnvironmentalSound", order =1)]
    public class EnvironmentalSound : ScriptableObject{
        
        [Header("Required")]
        public AudioClip clip;


        [Header("Optional")]
        public bool playOnce;
        public float volumeMod;
        public float pitchVar;
        public float fade;
        public bool randomPitchVariation;


        public bool pitchVaries {get {return pitchVar != 0;}}
        public bool hasFade {get {return fade > 0;}}

        
        public EnvironmentalSound(AudioClip clip, float volumeMod = 1, float pitchVar = 0, float fade = 1, bool randomPitchVar = false, AudioSource source = null, bool playOnce = false){
            this.clip = clip;
            this.volumeMod = volumeMod;
            this.pitchVar = pitchVar;
            this.fade = fade;
            this.randomPitchVariation = randomPitchVar;
            this.playOnce = playOnce;
        }
    }
}
