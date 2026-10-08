using System.Collections.Generic;
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

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayerMask;
    public bool isGrounded { get; private set; }
    private Vector3 groundNormal;
    [SerializeField] float probeForwardOffset;
    [SerializeField] float probeSideOffset;
    [SerializeField] float probeHeight;
    [SerializeField]private float groundProbeRadius;
    private RaycastHit[] groundHits;
    [SerializeField]private float groundProbeDistance;

    [Header("Slide")]
    private Vector3 slideVelocity;
    private float groundAngle;
    [SerializeField] private float slideSpeed = 25f;
    public bool isSliding { get; private set; }

    [Header("Overhead Collision")]
    private Vector3 collisionNormal;
    private bool hasCollision;

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
        groundHits = new RaycastHit[5];
        cameraTransform = Camera.main.transform;
        currentCoyoteTime = coyoteAirTime;
    }

    private void Update()
    {
        if (characterController.enabled)
        {
            hasCollision = false;
            collisionNormal = Vector3.zero;

            CheckGrounded();
            HandleMovement();
            HandleJump();
            ApplyGravity();

            Vector3 finalVelocity = horizontalVelocity;

            if (isGrounded && !hasJumped)
            {
                finalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, groundNormal);

                if (horizontalVelocity.sqrMagnitude > 0.001f)
                    finalVelocity = finalVelocity.normalized * horizontalVelocity.magnitude;
            }
            else if (isSliding && finalVelocity.y <= 0)
            {
                finalVelocity = Vector3.ProjectOnPlane(horizontalVelocity, groundNormal);

                Vector3 gravityVelocity = Vector3.ProjectOnPlane(Vector3.up * verticalVelocity, groundNormal);

                finalVelocity += gravityVelocity;
            }
            else
                finalVelocity.y = verticalVelocity;

            characterController.Move(finalVelocity * Time.deltaTime);

            Debug.Log($"isGrounded: {isGrounded} | isSliding: {isSliding}");
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
        if (CanJump() && PlayerInput.Instance.jumpAction.WasPressedThisFrame())
        {
            DataManager.Instance?.AddData(DataManager.Data.Jump, 1);
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            playerAnimatorController.PlayJump();

            hasJumped = true;
            currentCoyoteTime = 0f;
        }
        else
        {
            if (hasJumped && PlayerInput.Instance.jumpAction.WasReleasedThisFrame() && verticalVelocity > 0f)
            {
                verticalVelocity *= 0.5f;
            }
        }
    }

    private void ApplyGravity()
    {
        if(isGrounded && verticalVelocity < 0f)
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

    private bool CanJump()
    {
        if (isGrounded)
        {
            return true;
        }

        else if (!isGrounded && !hasJumped && currentCoyoteTime > 0f)
        {
            currentCoyoteTime -= Time.deltaTime;
            return true;
        }

        return false;
    }

    private void CheckGrounded()
    {
        debugHits.Clear();

        isGrounded = false;
        isSliding = false;

        groundNormal = Vector3.up;
        groundAngle = 0f;

        Vector3 feetPosition = transform.position + Vector3.up * probeHeight;

        Vector3[] probePositions =
            {
                feetPosition,
                feetPosition + transform.forward * probeForwardOffset,
                feetPosition - transform.forward * probeForwardOffset,
                feetPosition + transform.right * probeSideOffset,
                feetPosition - transform.right * probeSideOffset
            };

        float bestGroundAngle = float.MaxValue;
        Vector3 bestNormal = Vector3.up;

        foreach(Vector3 probePosition in probePositions)
        {
            int hitCount = Physics.SphereCastNonAlloc(
                probePosition,
                groundProbeRadius,
                Vector3.down,
                groundHits,
                groundProbeDistance,
                groundLayerMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = groundHits[i];

                if (hit.collider == null)
                    continue;

                debugHits.Add(hit);

                float angle = Vector3.Angle(hit.normal, Vector3.up);

                if (angle < bestGroundAngle)
                {
                    bestGroundAngle = angle;
                    bestNormal = hit.normal;
                }
            }
        }

        if (bestGroundAngle == float.MaxValue)
        {
            isGrounded = false;
            isSliding = false;
            return;
        }

        groundNormal = bestNormal;
        groundAngle = bestGroundAngle;

        if (groundAngle <= characterController.slopeLimit)
        {
            isGrounded = true;

            if (verticalVelocity <= 0f)
            {
                currentCoyoteTime = coyoteAirTime;
                hasJumped = false;
            }
        }
        else
        {
            isSliding = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0f)
        {
            hasCollision = true;
            collisionNormal = hit.normal;

            if (verticalVelocity > 0f)
                verticalVelocity = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (characterController == null)
            return;

        Vector3 feetPosition =
            transform.position +
            Vector3.up * probeHeight;

        Vector3[] probePositions =
        {
            feetPosition,

            feetPosition +
            transform.forward * probeForwardOffset,

            feetPosition -
            transform.forward * probeForwardOffset,

            feetPosition +
            transform.right * probeSideOffset,

            feetPosition -
            transform.right * probeSideOffset
        };

        foreach (Vector3 position in probePositions)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(
                position,
                groundProbeRadius
            );

            Gizmos.DrawLine(
                position,
                position + Vector3.down * groundProbeDistance
            );
        }
    }

    private Vector3[] debugProbePositions;
    private List<RaycastHit> debugHits = new List<RaycastHit>();

    private void OnDrawGizmos()
    {
        if (characterController == null)
            return;

        if (debugProbePositions != null)
        {
            foreach (Vector3 position in debugProbePositions)
            {
                Gizmos.color = Color.cyan;

                Gizmos.DrawWireSphere(
                    position,
                    groundProbeRadius
                );

                Gizmos.DrawLine(
                    position,
                    position + Vector3.down * groundProbeDistance
                );

                Gizmos.DrawWireSphere(
                    position + Vector3.down * groundProbeDistance,
                    groundProbeRadius
                );
            }
        }

        if (debugHits != null)
        {
            foreach (RaycastHit hit in debugHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.035f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(
                    hit.point,
                    hit.point + hit.normal * 0.3f
                );
            }
        }
    }
}