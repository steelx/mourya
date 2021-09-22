using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerControls inputActions;
    private Vector2 movementInput;
    private Vector2 cameraInput;

    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_input;// controler B button or Shift key
    private float rollInputTimer;
    public bool sprintFlag;// player going to spring
    public bool rollFlag;// player is going to roll

    // Awake happens before OnEnable

    // subscribe events
    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += (i) => movementInput = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += (i) => cameraInput = i.ReadValue<Vector2>();
        }
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // will be called inside Player script Update
    public void TickInput(float delta)
    {
        MoveInput(delta);
        RollingInput(delta);
    }
    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void RollingInput(float delta)
    {
        b_input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        if (b_input)
        {
            rollInputTimer += delta;
            sprintFlag = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }

    }
}
