using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public static string currentAnimation = "";
    public static string previousAnimation = "still"; // Default animation


    private MouseMovement mouse;

    // Start is called before the first frame update
    void Start()
    {
        mouse = GetComponent<MouseMovement>();
        animator = GetComponent<Animator>();
    }

    public void ChangeAnimation(string animationName, float crossfade = 0.2f, float time = 0.0f)
    {
        if (time > 0.0f)
            StartCoroutine(Wait());
        else
            Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossfade);
            Validate();
        }

        void Validate()
        {
            if (currentAnimation != animationName)
            {
                previousAnimation = currentAnimation;
                currentAnimation = animationName;

                if (currentAnimation == "")
                    mouse.CheckAnimations();
                else
                    animator.CrossFade("Mouse" + animationName, crossfade);
            }
        }
    }

    public float GetAnimationLength(string animationName, float crossfade = 0.2f)
    {
        if (animator == null) return 0f;

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == "Mouse" + animationName)
            {
                //Debug.Log("clip length is " + clip.length);
                return clip.length - crossfade;
            }
        }

        return 0f; // Default fallback
    }
}
