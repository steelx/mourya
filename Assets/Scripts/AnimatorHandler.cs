using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorHandler : MonoBehaviour
{

    [field: SerializeField]
    public bool CanRotate { set; get; }
    public Animator anim;
    int horizontal;
    int vertical;

    private CharacterLocomotion playerLocomotion;

    private void Start()
    {
        playerLocomotion = GetComponentInParent<CharacterLocomotion>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    private void OnAnimatorMove()
    {
        if (!IsInteracting()) return;

        //playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / Time.deltaTime;
        playerLocomotion.chController.SimpleMove(velocity);
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        float delta = Time.deltaTime;
        #region Vertical
        float v = 0;
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1f;
        }
        else
        {
            v = 0;
        }
        #endregion

        #region Horizontal
        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1f;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1f;
        }
        else
        {
            h = 0;
        }
        #endregion

        if (isSprinting)
        {
            v = 2f;
            h = horizontalMovement;
        }

        anim.SetFloat(vertical, v, 0.1f, delta);
        anim.SetFloat(horizontal, h, 0.1f, delta);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnimation, 0.2f);
    }

    public bool IsInteracting()
    {
        return anim.GetBool("isInteracting");
    }
}
