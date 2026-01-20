using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("References (Required)")]
    [Tooltip("Camera used to calculate movement direction")]
    public Transform cameraTransform;

    [Header("Movement - Basic")]
    public bool AllowMovement = true;
    public float walkSpeed = 6f;
    public bool allowRunning = true;
    public float runSpeed = 10f;

    [Header("Movement - Advanced")]
    public float acceleration = 12f;

    [Range(0f, 1f)]
    public float airControlMultiplier = 0.5f;

    public bool moveRelativeToCamera = true;

    [Tooltip("Character rotates to face movement direction")]
    public bool rotateTowardsMovement = true;

    [Tooltip("How fast the character rotates")]
    public float rotationSpeed = 12f;

    [Header("Jump - Basic")]
    public bool allowJump = true;
    public float jumpForce = 7f;

    [Header("Jump - Advanced")]
    public float gravity = -20f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Slope Adaptation")]
    public bool adaptToSlope = true;
    public Transform visual;
    public float slopeRotationSpeed = 10f;
    public float maxSlopeAngle = 45f;

    // Editor foldouts
    [HideInInspector] public bool _editorFoldoutReferences = true;
    [HideInInspector] public bool _editorFoldoutMovement = true;
    [HideInInspector] public bool _editorFoldoutMovementAdvanced;
    [HideInInspector] public bool _editorFoldoutRotation = true;
    [HideInInspector] public bool _editorFoldoutJump = true;
    [HideInInspector] public bool _editorFoldoutJumpAdvanced;
    [HideInInspector] public bool _editorFoldoutControls;
    [HideInInspector] public bool _editorFoldoutSlope;

    // Internal
    CharacterController controller;
    Vector3 currentVelocity;
    Vector3 moveDirection;
    float verticalVelocity;

    float lastGroundedTime;
    float lastJumpInputTime;

    public Vector3 Velocity => currentVelocity;
    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded => controller.isGrounded;
    public bool IsRunning { get; private set; }

    RaycastHit slopeHit;

    PlayerInputActions input;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (adaptToSlope && visual == null)
        {
            Debug.LogError(
                "[CharacterMovement] Adapt To Slope is enabled but no Visual is assigned.",
                this
            );
        }

        input = new PlayerInputActions();
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        HandleTimers();
        if(AllowMovement) HandleMovement();
        HandleRotation();
        HandleGravityAndJump();
        HandleSlopeAdaptation();
    }

    void HandleTimers()
    {
        if (controller.isGrounded)
            lastGroundedTime = Time.time;

        if (input.Player.Jump.WasPressedThisFrame())
            lastJumpInputTime = Time.time;
    }

    void HandleMovement()
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        IsRunning = allowRunning && input.Player.Run.IsPressed();

        float targetSpeed = IsRunning ? runSpeed : walkSpeed;

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        moveDirection = Vector3.zero;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            if (moveRelativeToCamera && cameraTransform != null)
            {
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;

                forward.y = 0f;
                right.y = 0f;

                moveDirection =
                    (forward.normalized * inputDir.z +
                     right.normalized * inputDir.x).normalized;
            }
            else
            {
                moveDirection = transform.TransformDirection(inputDir);
            }
        }

        float control = controller.isGrounded ? 1f : airControlMultiplier;

        Vector3 desiredVelocity = moveDirection * targetSpeed;
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            desiredVelocity,
            acceleration * control * Time.deltaTime
        );

        controller.Move(currentVelocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (!rotateTowardsMovement)
            return;

        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleGravityAndJump()
    {
        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        bool canJump =
            allowJump &&
            (Time.time - lastGroundedTime <= coyoteTime) &&
            (Time.time - lastJumpInputTime <= jumpBufferTime);

        if (canJump)
        {
            verticalVelocity = jumpForce;
            lastJumpInputTime = -10f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    void HandleSlopeAdaptation()
    {
        if (!adaptToSlope || visual == null)
            return;

        Quaternion uprightRotation =
            Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Quaternion targetRotation = uprightRotation;

        if (controller.isGrounded)
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out slopeHit, 2f))
            {
                float angle = Vector3.Angle(slopeHit.normal, Vector3.up);

                if (angle <= maxSlopeAngle)
                {
                    targetRotation =
                        Quaternion.FromToRotation(Vector3.up, slopeHit.normal) *
                        uprightRotation;
                }
            }
        }

        visual.rotation = Quaternion.Slerp(
            visual.rotation,
            targetRotation,
            slopeRotationSpeed * Time.deltaTime
        );
    }

    public void allowController(bool allow){
        AllowMovement = allow;
        allowJump = allow;
    }
}
