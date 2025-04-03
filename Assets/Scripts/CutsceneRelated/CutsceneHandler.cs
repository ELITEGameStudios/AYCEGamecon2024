using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    private PlayerMovement player;
    private PlayerAnimations animations;
    private Rigidbody2D rb;

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

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        animations = FindObjectOfType<PlayerAnimations>();
        rb = player.GetComponent<Rigidbody2D>();
        originalGrav = rb.gravityScale;
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
        else if (cutsceneName == mouse)
        {
            //PlayMouseCutscene();
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
        Time.timeScale = slowMotionTimeScale;
        rb.gravityScale = newGravScale;
        
        Invoke(nameof(StandingUp), timeToStandUp * slowMotionTimeScale);
        //player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void StandingUp()
    {
        animations.ChangeAnimation("Standing");
        Time.timeScale = 1; // Restore normal time
        rb.gravityScale = originalGrav; // Restore normal gravity
        player.GetComponent<PlayerMovement>().enabled = false; // Re-enable movement
    }
}
