using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    private PlayerMovement player;
    private PlayerAnimations animations;
    private CameraFollowScript cameraFollowScript;
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private GameObject cutsceneZones, postCutZones, mousePrefab;
    [SerializeField] private MouseNodes mouseNode;
    [SerializeField] private AudioClip fallAudioClip;
    [SerializeField] private AudioSource fallAudioSource;

    [SerializeField] private string cutsceneName = "";
    private string landfill = "StartingCutscene";
    private string lab = "GrandFallTrigger";
    private string mouse = "MouseTrigger";
    private string boss = "BossCutscene";
    private string animatic = "animaticCutscene";

    [SerializeField] private float timeToStandUp = 1.5f;
    [SerializeField] private float slowMotionTimeScale = 0.2f;
    [SerializeField] private float newGravScale = 0.2f;
    private float originalGrav;
    private bool playedMouseCutscene;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        animations = FindObjectOfType<PlayerAnimations>();
        rb = player.GetComponent<Rigidbody2D>();
        originalGrav = rb.gravityScale;
        cameraFollowScript = FindObjectOfType<CameraFollowScript>();
        animator = player.GetComponentInChildren<Animator>();
    }

    public void StartCutscene(string cutsceneName)
    {
        if (cutsceneName == lab)
        {
            PlayLabCutscene();
        }
        else if (cutsceneName == landfill)
        {
            //PlayLandfillCutscene();
        }
        else if (cutsceneName == mouse && !playedMouseCutscene)
        {
            PlayMouseCutscene();
        }
        else if (cutsceneName == boss)
        {
            //PlayBossCutscene();
        }
        else if (cutsceneName == animatic)
        {
            //PlayAnimaticCutscene();
        }
    }

    private void PlayLabCutscene()
    {
        player.GetComponent<PlayerMovement>().enabled = false;

        animations.ChangeAnimation("HorizontalFalling");
        
        //fallAudioSource.clip = fallAudioClip;
        //fallAudioSource.volume = AudioSystem.volume;
        //fallAudioSource.Play();

        //camera adjust

        Time.timeScale = slowMotionTimeScale;
        rb.gravityScale = newGravScale;
        
        Invoke(nameof(StandingUp), timeToStandUp * slowMotionTimeScale);
        //player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void PlayMouseCutscene()
    {
        playedMouseCutscene = true;
        player.GetComponent<PlayerMovement>().enabled = false;
        GameObject mouse = Instantiate(mousePrefab, mouseNode.transform.position, mouseNode.transform.rotation);

        mouse.GetComponent<MouseAnimations>().ChangeAnimation("Burrowing");
        animations.ChangeAnimation("MouseMeet");
        
        Invoke(nameof(ReenableMovement), 4);
    }

    private void ReenableMovement(){
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void StandingUp()
    {
        CameraFollowScript.Instance.Shake(5, 2.5f, 1);
        animations.ChangeAnimation("Standing");
        Time.timeScale = 1; // Restore normal time
        rb.gravityScale = originalGrav; // Restore normal gravity

        // Wait for animation to finish before enabling movement
        StartCoroutine(WaitForAnimation("Standing"));
    }

    private IEnumerator WaitForAnimation(string animationName)
    {
        while (PlayerAnimations.currentAnimation == "Standing")
        {
            yield return null; // Wait until the animation is done
        }

        player.GetComponent<PlayerMovement>().enabled = true; // Re-enable movement
        cutsceneZones.SetActive(false);
        postCutZones.SetActive(true);
    }
}
