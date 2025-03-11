using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animatorVV;
    [SerializeField] private Animator animatorChest;
    [SerializeField] private Animator animatorBoots;
    [SerializeField] private Animator animatorHands;
    [SerializeField] private Animator animatorEyes;

    public static string currentAnimation = "";

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        //ChangeAnimation("Jumping");
    }

    void Update()
    {
        
    }

    public void ChangeAnimation(string animationName, float crossfade = 0.2f, float time = 0.0f) //try StringToHash for optimization
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
                currentAnimation = animationName;

                if (currentAnimation == "")
                    playerMovement.CheckAnimations();
                else
                {
                    animatorVV.CrossFade("VV" + animationName, crossfade);
                    animatorChest.CrossFade("Chest" + animationName, crossfade);
                    animatorBoots.CrossFade("Boots" + animationName, crossfade);
                    animatorHands.CrossFade("Hands" + animationName, crossfade);
                    animatorEyes.CrossFade("Eyes" + animationName, crossfade);
                }
            }
        }
    }

    
}
