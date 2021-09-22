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
    private Vector3 playerVelocity;

    [HideInInspector]
    public AnimatorHandler animatorHandler;


    [Header("Fall Detections Stats")]
    float leapingVelocity = 0.3f;
    [SerializeField]
    LayerMask groundLayer;
    float inAirTimer;


    [Header("Movement Stats")]
    const float movementSpeed = 3f;
    const float walkingSpeed = 1.8f;
    const float sprintSpeed = 8f;
    const float rotationSpeed = 10;
    //[SerializeField] float jumpHeight = 1.0f;
    const float gravityValue = -9.81f;
    const float fallingSpeed = 25f;
    const float groundDirectionRayDistance = 0.5f;


    //Player Flags
    public bool isGrounded;
    public bool isSprinting;
    public bool isInAir;
    [HideInInspector]
    public bool isJumping;


    // Start is called before the first frame update
    void Start()
    {
        chController = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        isInAir = !chController.isGrounded;
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
        if (chController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }
        isSprinting = inputHandler.b_input;
        if (inputHandler.rollFlag) return;

        float speed = walkingSpeed;
        if (inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
            isSprinting = true;
        }

        Vector2 inputXY = new Vector2(inputHandler.horizontal, inputHandler.vertical);
        moveDirection = getFollowCameraMovement(inputXY, delta);
        moveDirection *= delta * speed;

        chController.Move(moveDirection);

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
        playerVelocity.y += gravityValue * delta;
        chController.Move(playerVelocity);
        Debug.LogWarning("playerVelocity: " + playerVelocity);

        //animation
        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);
        if (animatorHandler.CanRotate)
        {
            HandleRotation(delta);
        }
    }
    Vector3 getFollowCameraMovement(Vector2 input, float delta)
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
            moveDirection = getFollowCameraMovement(inputXY, delta);

            //moveDirection = cameraObject.forward * inputHandler.vertical;
            //moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                //this.transform.rotation = rollRotation;
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
        isInAir = !chController.isGrounded;
        isGrounded = chController.isGrounded;
        if (isInAir)
        {
            inAirTimer += delta;
        }
    }

    public void HandleFallingAndLanding(float delta)
    {
        if (isInAir)
        {
            chController.Move(Vector3.down * fallingSpeed);
            chController.Move(moveDirection * leapingVelocity * fallingSpeed);// jump off edge
        }

        Vector3 raycastOrigin = chController.transform.position;
        Vector3 targetPosition = chController.transform.position;
        Vector3 dir = moveDirection;
        dir.Normalize();
        raycastOrigin += dir * groundDirectionRayDistance;

        // this checks if we are on ground, and we `hit` ground layer
        if (Physics.Raycast(raycastOrigin, Vector3.down, out RaycastHit hit, 1, groundLayer))
        {
            // if raycast hits somthing, means we are on ground
            // we also need to calculate movement here, since CharacterMovement will be locked 
            // untill we are isInAir = true

            //if (isInAir && !animatorHandler.IsInteracting())
            //{
            //    animatorHandler.PlayTargetAnimation("Land", true);
            //}

            //Debug.LogWarning("Hit " + hit.distance);
            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;

            // if were inAir during detection
            if (isInAir)
            {
                Debug.DrawRay(raycastOrigin, Vector3.down, Color.green);
                if (inAirTimer > 0.5f || hit.distance > 0.5f)
                {
                    Debug.Log("You were in air for: " + inAirTimer);
                    animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Locomotion", false);
                    inAirTimer = 0;
                }
                
                isInAir = false;
            }
        }
        else
        {
            // didnt hit ground layer
            if (!isInAir)
            {
                if (!animatorHandler.IsInteracting())
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                Vector3 velocity = chController.velocity;
                velocity.Normalize();
                chController.Move(velocity * (movementSpeed/2));
                isInAir = true;
            }
        }

        if (chController.isGrounded)
        {
            if (animatorHandler.IsInteracting() || inputHandler.moveAmount > 0)
            {
                chController.transform.position = Vector3.Lerp(chController.transform.position, targetPosition, delta / 0.1f);
            }
            else
            {
                chController.transform.position = targetPosition;
            }
        }
    }

    public void CharacterFallingAndLanding(float delta)
    {
        if (isInAir)
        {
            chController.Move(delta * fallingSpeed * Vector3.down);
            chController.Move(fallingSpeed * leapingVelocity * moveDirection);// jump off edge
        }


    }
}
