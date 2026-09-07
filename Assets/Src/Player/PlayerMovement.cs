using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (characterController.enabled)
        {
            HandleMovement();
            HandleJump();
            ApplyGravity();

            Vector3 finalVelocity = horizontalVelocity;
            finalVelocity.y = verticalVelocity;

            characterController.Move(finalVelocity * Time.deltaTime);
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

        Vector3 targetVelocity = moveDirection * (maxSpeed * inputMagnitude);

        float accelerationRate = inputMagnitude > 0.01f ? acceleration : deceleration;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accelerationRate * Time.deltaTime);

        if(moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);

            float currentRotationSpeed = rotationSpeed;
            if (angle > 120f)
                currentRotationSpeed *= 1.5f;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (PlayerInput.Instance.jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                playerAnimatorController.PlayJump();
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
        verticalVelocity += gravity * Time.deltaTime;
    }

    public void Launch()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
