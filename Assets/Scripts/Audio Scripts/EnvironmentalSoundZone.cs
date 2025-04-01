using UnityEngine;


namespace SoundSystems{
    public class EnvironmentalSoundZone : MonoBehaviour{
        [SerializeField] private EnvSoundAccessor[] accessors;
        [SerializeField] private bool hasPlayer;

        void OnTriggerEnter2D(Collider2D col){
            if(col == Player.main.MainCol){
                hasPlayer = true;
                CheckAccesors(true);
                Debug.Log("Entering sound zone");
            }
        }
        void OnTriggerExit2D(Collider2D col){
            if(col == Player.main.MainCol){
                hasPlayer = false;
                CheckAccesors(false);
                Debug.Log("leaving Sound Zone");
            }
        }

        void CheckAccesors(bool entered){
            foreach (EnvSoundAccessor accessor in accessors){
                EnvSoundInstance envSound; 
                if(entered){
                    if(accessor.playsOnlyInZone || accessor.stopPlaying){
                        envSound = EnvironmentalSoundSystem.instance.FindEnvSound(accessor, gameObject);
                        if(envSound != null) envSound.Play();
                    }
                }
                else{
                    if(accessor.playsOnlyInZone || accessor.stopPlaying){
                        envSound = EnvironmentalSoundSystem.instance.FindEnvSound(accessor, gameObject);
                        if(envSound != null) envSound.Stop();
                    }
                }
            }
        }
    }
}
