using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraObject;
    InputHandler inputHandler;
    public Vector3 moveDirection;

    [HideInInspector]
    public new Rigidbody rigidbody;

    [HideInInspector]
    public Transform playerTransform;
    [HideInInspector]
    public AnimatorHandler animatorHandler;


    [Header("Fall Detections Stats")]
    [SerializeField]
    float raycastHeightOffset = 0.5f;
    [SerializeField]
    float leapingVelocity = 3f;
    [SerializeField]
    LayerMask groundLayer;
    float inAirTimer;


    [Header("Movement Stats")]
    [SerializeField]
    const float movementSpeed = 3f;
    [SerializeField]
    const float walkingSpeed = 1.8f;
    [SerializeField]
    const float sprintSpeed = 8f;
    [SerializeField]
    const float rotationSpeed = 10;
    [SerializeField]
    const float fallingSpeed = 45;


    //Player Flags
    [HideInInspector]
    public bool isSprinting;
    [HideInInspector]
    public bool isInAir;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isJumping;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        playerTransform = this.transform;
        isGrounded = true;
    }

    #region Movement
    Vector3 normalVector;

    void HandleRotation(float delta)
    {
        Vector3 targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = playerTransform.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(playerTransform.rotation, rotation, rotationSpeed * delta);

        playerTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        isSprinting = inputHandler.b_input;
        if (inputHandler.rollFlag) return;
        if (isInAir) return;

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0; // FIX walking on air due to camera

        float speed = walkingSpeed;
        if (inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
            isSprinting = true;
        }
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);
        if (animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSprint(bool isInteracting)
    {
        if (isInteracting) return;

        if(inputHandler.rollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                this.transform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    #endregion

    public void UpdateInAirTimer(float delta)
    {
        if (isInAir)
        {
            inAirTimer += delta;
        }
    }


    public void HandleFallingAndLanding(float delta)
    {
        RaycastHit hit;
        Vector3 raycastOrigin = this.transform.position;
        Vector3 targetPosition = this.transform.position;
        raycastOrigin.y += raycastHeightOffset;

        if (!isGrounded && !isJumping)
        {
            if (!animatorHandler.IsInteracting())
            {
                animatorHandler.PlayTargetAnimation("Falling", true);
            }

            inAirTimer += delta;
            rigidbody.AddForce(this.transform.forward * leapingVelocity);
            rigidbody.AddForce(-Vector3.up * fallingSpeed * inAirTimer);// jump off edge
        }

        if(Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, 10, groundLayer))
        {
            if (!isGrounded && !animatorHandler.IsInteracting())
            {
                animatorHandler.PlayTargetAnimation("Land", true);
            }

            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            if (animatorHandler.IsInteracting() || inputHandler.moveAmount > 0)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, delta / 0.1f);
            }
            else
            {
                this.transform.position = targetPosition;
            }
        }
    }

}
