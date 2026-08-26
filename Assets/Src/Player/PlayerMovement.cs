using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction jumpAction;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float gravity = -25f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player");
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();

    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();

        Vector3 finalVelocity = horizontalVelocity;
        finalVelocity.y = verticalVelocity;

        characterController.Move(finalVelocity * Time.deltaTime);

        UpdateAnimator();
    }

    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

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

            if (jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }

        }
    }

    private void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        float speed = horizontalVelocity.magnitude;
        bool isGrounded = characterController.isGrounded;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
    }
}
