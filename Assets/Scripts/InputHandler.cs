using UnityEngine;

public class InputHandler : MonoBehaviour
{
    PlayerControls inputActions;
    PlayerAttacks playerAttacks;
    PlayerInventory playerInventory;
    Vector2 movementInput;
    Vector2 cameraInput;

    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_input;// controler B button or Shift key
    public bool rb_input;
    public bool rt_input;
    private float rollInputTimer;
    public bool sprintFlag;// player going to spring
    public bool rollFlag;// player is going to roll

    // Awake happens before OnEnable
    private void Awake()
    {
        playerAttacks = GetComponent<PlayerAttacks>();
        playerInventory = GetComponent<PlayerInventory>();
        inputActions = new PlayerControls();
        inputActions.PlayerMovement.Movement.performed += (i) => movementInput = i.ReadValue<Vector2>();
        inputActions.PlayerMovement.Camera.performed += (i) => cameraInput = i.ReadValue<Vector2>();
    }

    // subscribe events
    private void OnEnable()
    {
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
        AttackInput(delta);
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

    private void AttackInput(float delta)
    {
        inputActions.PlayerActions.RB.performed += i => rb_input = true;
        inputActions.PlayerActions.RT.performed += i => rt_input = true;

        if (rb_input)
        {
            playerAttacks.HandleLightAttack(playerInventory.rightWeapon);
        }
        if (rt_input)
        {
            playerAttacks.HandleHeavyAttack(playerInventory.leftWeapon);
        }
    }
}
