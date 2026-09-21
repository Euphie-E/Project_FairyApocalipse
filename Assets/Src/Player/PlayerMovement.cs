using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    private PlayerAnimatorController playerAnimatorController;
    public CharacterController characterController { get; private set; }

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float gravity = -25f;
    public Vector3 horizontalVelocity { get; private set; }
    public float verticalVelocity { get; private set; }

    [Header("Slide")]
    public bool isGrounded { get; private set; }
    private Vector3 groundNormal;
    private float groundAngle;
    [SerializeField] private float slideSpeed = 25f;

    [Header("Coyote Jump")]
    [SerializeField] private float coyoteAirTime = 0.5f;
    private float currentCoyoteTime;
    private bool hasJumped;

    private void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        currentCoyoteTime = coyoteAirTime;
    }

    private void Update()
    {
        if (characterController.enabled)
        {
            

            HandleMovement();
            HandleJump();
            ApplyGravity();

            Vector3 finalVelocity = horizontalVelocity;

            if (!isGrounded && characterController.velocity.y < 0)
            {
                finalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, groundNormal);

                finalVelocity += GetSlideVelocity();
            }

            finalVelocity.y = verticalVelocity;

            characterController.Move(finalVelocity * Time.deltaTime);
            //Debug.Log(isGrounded);
            //Debug.Log(finalVelocity);
        }
    }

    private void HandleMovement()
    {
        Vector2 input = PlayerInput.Instance.moveAction.ReadValue<Vector2>();

        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = right * input.x + forward * input.y;
        if(moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);

            float currentRotationSpeed = rotationSpeed;

            if (angle > 120f)
                currentRotationSpeed *= 1.5f;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
        }

        Vector3 targetVelocity = moveDirection * (maxSpeed * inputMagnitude);

        float accelerationRate = inputMagnitude > 0.01f ? acceleration : deceleration;

        float directionDot = 0f;

        if (horizontalVelocity.sqrMagnitude > 0.01f && moveDirection.sqrMagnitude > 0.01f)
        {
            directionDot = Vector3.Dot(horizontalVelocity.normalized, moveDirection);
        }

        if (directionDot < 0.5f)
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, deceleration * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (CanJump())
        {
            if (PlayerInput.Instance.jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                playerAnimatorController.PlayJump();
                hasJumped = true;
            }
        }
        else
        {
            if (PlayerInput.Instance.jumpAction.WasReleasedThisFrame() && verticalVelocity > 0f)
            {
                verticalVelocity *= 0.5f;
            }
        }
    }

    private void ApplyGravity()
    {
        if(characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
            return;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    public void Launch()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if(hit.normal.y > 0f)
        {
            groundNormal = hit.normal;
            groundAngle = angle;

            if (groundAngle <= characterController.slopeLimit)
                isGrounded = true;
            else
                isGrounded = false;
        }
    }

    private Vector3 GetSlideVelocity()
    {
        Vector3 slideDirection = Vector3.ProjectOnPlane(
            Vector3.down,
            groundNormal
        ).normalized;

        return slideDirection * slideSpeed;
    }

    private bool CanJump()
    {
        if (isGrounded && characterController.isGrounded)
        {
            hasJumped = false;
            currentCoyoteTime = coyoteAirTime;
            return true;
        }
        else if (!characterController.isGrounded && !hasJumped && currentCoyoteTime >= 0f)
        {
            currentCoyoteTime -= Time.deltaTime;
            return true;
        }
        else 
        {
            return false;
        }
    }
}