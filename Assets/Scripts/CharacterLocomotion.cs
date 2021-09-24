using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterLocomotion : MonoBehaviour
{
    Transform cameraObject;
    InputHandler inputHandler;
    public Vector3 moveDirection;

    [HideInInspector]
    public CharacterController chController;
    public Vector3 playerVelocity;

    [HideInInspector]
    public AnimatorHandler animatorHandler;


    [Header("Fall Detections Stats")]
    [SerializeField] LayerMask groundLayer;
    float inAirTimer;


    [Header("Movement Stats")]
    const float walkingSpeed = 2.4f;
    const float sprintSpeed = 8.81f;
    const float rotationSpeed = 10;
    const float gravityValue = -9.81f;
    //[SerializeField] float jumpHeight = 1.0f;


    //Player Flags
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isInAir;
    [HideInInspector] public bool isJumping;


    // Start is called before the first frame update
    void Start()
    {
        chController = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        isGrounded = true;
    }

    void HandleRotation(float delta)
    {
        Vector3 targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = chController.transform.forward;
        }

        Quaternion rotation = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(chController.transform.rotation, rotation, rotationSpeed * delta);

        chController.transform.rotation = targetRotation;
    }

    public void CharacterMovement(float delta)
    {
        float speed = walkingSpeed;
        if (inputHandler.sprintFlag & inputHandler.moveAmount > 0.5)
        {
            speed = sprintSpeed;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        Vector2 inputXY = new Vector2(inputHandler.horizontal, inputHandler.vertical);
        moveDirection = GetFollowCameraMovement(inputXY);
        moveDirection *= speed;
        chController.Move(delta * moveDirection);

        // rotation
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * delta);
            chController.transform.rotation = targetRotation;
        }

        // Changes the height position of the player..
        //if (jumpAction.triggered && chController.isGrounded)
        //{
        //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        //}

        // Gravity
        if (chController.isGrounded || playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }
        else
        {
            playerVelocity.y += gravityValue * delta;
        }

        chController.Move(playerVelocity * delta);


        //animation
        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);
        if (animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }
    Vector3 GetFollowCameraMovement(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraObject.right.normalized + move.z * cameraObject.forward.normalized;
        move.y = 0;
        return move;
    }

    public void HandleRollingAndSprint(float delta)
    {
        if (animatorHandler.IsInteracting()) return;

        if (inputHandler.rollFlag)
        {
            Vector2 inputXY = new Vector2(inputHandler.horizontal, inputHandler.vertical);
            moveDirection = GetFollowCameraMovement(inputXY);

            if (inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                chController.transform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    public void UpdateInAirTimer(float delta)
    {
        if (isInAir)
        {
            inAirTimer += delta;
        }
    }

    public void CharacterFallingAndLanding(float delta)
    {
        Vector3 origin = chController.transform.position;
        // using late update here
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hitInfo, 1f, groundLayer))
        {
            isGrounded = true;
            // if were inAir during detection
            if (isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    Debug.Log("You were in air for: " + inAirTimer);
                    animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Locomotion", false);
                }
                inAirTimer = 0;
                isInAir = false;
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
            }
            if (!isInAir)
            {
                if (!animatorHandler.IsInteracting())
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }
                isInAir = true;
            }
        }
    }
}
