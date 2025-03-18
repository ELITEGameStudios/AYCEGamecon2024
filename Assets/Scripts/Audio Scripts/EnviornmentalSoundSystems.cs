
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystems{
    public class EnvironmentalSoundSystem : MonoBehaviour{
        public bool playSoundsOnStartup;
        public List<EnvSoundInstance> sounds;
        public static EnvironmentalSoundSystem instance {get; private set;}


        void Awake(){
            if(instance == null){instance = this;}
            else if(instance != this){Destroy(this);}
            DontDestroyOnLoad(this);
            
            sounds ??= new();

            if(sounds.Count > 0) {
                foreach (EnvSoundInstance sound in sounds){
                    sound.Initialize();
                    if(playSoundsOnStartup) sound.Play();
                }
            }
        }

        public EnvSoundInstance FindEnvSound(EnvSoundAccessor accessor){
            // If sound exists
            foreach (EnvSoundInstance sound in sounds){
                if(accessor.name == sound.name){ return sound; }
            }
            
            // If sound does not exist
            if(accessor.canCreate){
                EnvSoundInstance newSound = new EnvSoundInstance(accessor.name, accessor.createData);
                sounds.Add(newSound);
                return newSound;
            }
            return null;
        }

        public EnvSoundInstance FindEnvSound(string name){
            // If sound exists
            foreach (EnvSoundInstance sound in sounds){
                if(name == sound.name){ return sound; }
            }
            return null;
        }

        public void CreateEnvSound(string name, EnvironmentalSound soundData, bool playImmediately = true, GameObject soundOrgin = null){
            // Check if sound already exists and not in use
            EnvSoundInstance newSound = FindEnvSound(name);
            if(newSound != null && !newSound.active){
                if(playImmediately){
                    newSound.Play();
                    return;
                }
            } 

            // Otherwise creates a new one
            newSound = new EnvSoundInstance(name, soundData, soundOrgin);
            sounds.Add(newSound);
            if(playImmediately){newSound.Play();}
        }

        void FixedUpdate(){
            foreach (EnvSoundInstance sound in sounds){
                sound.Update();
            }
        }



    }

    [System.Serializable]
    public class EnvSoundInstance{
        
        [Header("Required")]
        [SerializeField] private EnvironmentalSound soundData;
        public string name;

        [Header("Optional")]
        public AudioSource source;
        public GameObject obj;
        
        [Header("Extra Info")]
        public bool active;
        public float timer;

        public EnvSoundInstance(string name, EnvironmentalSound soundData, GameObject obj = null){
            this.name = name;
            this.soundData = soundData;
            this.obj = obj;
            this.soundData = soundData;
            Initialize();
        }


        public void Initialize(){
            timer = 0;
            active = false;
            obj = obj != null ? obj : EnvironmentalSoundSystem.instance.gameObject;

            // Source configuration
            if(source == null) source = obj.AddComponent<AudioSource>();
            
            source.clip = soundData.clip;
            source.volume = AudioSystem.volume * soundData.volumeMod;
            source.loop = !soundData.playOnce;
            source.spatialBlend = 0;
            source.spatialize = true;
            source.playOnAwake = soundData.playOnce;
            if(soundData.pitchVaries){
                source.pitch = 1 + (soundData.randomPitchVariation ? Random.Range(-soundData.pitchVar, soundData.pitchVar) : soundData.pitchVar);
            }
        }

        public void Update(){
            if(soundData.playOnce){
                if(!source.isPlaying){
                    active = false;
                    timer = 0;
                    source.volume = 0;
                }
            }

            if(active){
                if(soundData.hasFade && timer < soundData.fade){
                    timer += Time.fixedUnscaledDeltaTime;
                    source.volume = soundData.volumeMod * AudioSystem.volume * timer/soundData.fade;
                }
                else{
                    source.volume = soundData.volumeMod * AudioSystem.volume;
                }
            }
            else{
                if(soundData.hasFade && timer > 0){
                    timer -= Time.fixedUnscaledDeltaTime;
                    source.volume = soundData.volumeMod  * AudioSystem.volume * timer/soundData.fade;
                }
                else if(source.isPlaying){
                    source.Stop();
                    source.volume = 0;
                }
            }
        }

        public void Stop(){
            if(!active) return;
            active = false;
        }

        public void Play(){
            if(active) return;
            active = true;
            
            if(!source.isPlaying){
                if(soundData.pitchVaries){
                    source.pitch = 1 + (soundData.randomPitchVariation ? Random.Range(-soundData.pitchVar, soundData.pitchVar) : soundData.pitchVar);
                }

                source.Play();
            }
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName ="New Environmental Sound", menuName ="Environmental Sound", order =1)]
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

    [System.Serializable]
    public struct EnvSoundAccessor{
        public string name;
        public bool startPlaying;
        public bool stopPlaying;
        public EnvironmentalSound createData;


        public bool canCreate {get{return createData != null;}} // If you only want audio to play in the zone, set startPlaying and stopPlaying to false
        public bool playsOnlyInZone {get{return !startPlaying && !stopPlaying;}} // If you only want audio to play in the zone, set startPlaying and stopPlaying to false
        public EnvSoundAccessor(string name, bool startPlaying = false, bool stopPlaying = false, EnvironmentalSound createData = null){
            this.name = name;
            this.startPlaying = startPlaying;
            this.stopPlaying = stopPlaying;
            this.createData = createData;
        }
    }
}
