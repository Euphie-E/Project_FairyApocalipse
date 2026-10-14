using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IMushroomBounceReceiver
{
    [Header("Footsteps Audio")]
    [SerializeField] private float stepInterval = 0.4f;
    private float stepTimer;
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
    [SerializeField] private float maxGravityVelocity = -10f;
    public Vector3 horizontalVelocity { get; private set; }
    public float verticalVelocity { get; private set; }

    private bool mushroomBounceActive;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float snapDistance = 0.4f;
    [SerializeField] private float groundRadiusShrink = 0.05f;
    private Vector3 groundNormal = Vector3.up;
    private float groundAngle;
    public bool isGrounded { get; private set; }
    public bool isSliding { get; private set; }

    [Header("Coyote Jump")]
    [SerializeField] private float coyoteTime = 0.12f;
    private float lastGroundedTime = -10f;
    private float lastJumpTime = -10f;
    private const float ignoreGroundAfterJump = 0.1f;
    private bool hasJumped;

    private void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (!characterController.enabled) return;

        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleFootsteps();

        Vector3 velocity = CalculateFinalVelocity();
        bool groundedBeforeMove = isGrounded;

        CollisionFlags flags = characterController.Move(velocity * Time.deltaTime);

        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f) //Bateu no teto
            verticalVelocity = 0f;

        if (groundedBeforeMove && !characterController.isGrounded)
            SnapToGround();

#if UNITY_EDITOR
        debugFinalVelocity = velocity;
#endif
    }

    private Vector3 CalculateFinalVelocity()
    {
        if (isGrounded && verticalVelocity <= 0f)
        {
            Vector3 v = Vector3.ProjectOnPlane(horizontalVelocity, groundNormal);
            if (v.sqrMagnitude > 0.0001f)
                v = v.normalized * horizontalVelocity.magnitude;

            v.y += verticalVelocity;
            return v;
        }

        if (isSliding)
        {
            Vector3 v = Vector3.ProjectOnPlane(horizontalVelocity, groundNormal);
            v += Vector3.ProjectOnPlane(Vector3.up * verticalVelocity, groundNormal);
            return v;
        }

        Vector3 air = horizontalVelocity;
        air.y = verticalVelocity;
        return air;
    }

    private void SnapToGround()
    {
        if (hasJumped || verticalVelocity > 0f) return;

        if (ProbeGround(snapDistance, true, out RaycastHit hit, out Vector3 normal) && 
            Vector3.Angle(normal, Vector3.up) <= characterController.slopeLimit)
        {
            float drop = hit.distance - 0.1f - groundRadiusShrink;
            if (drop > 0.001f)
                characterController.Move(Vector3.down * drop);
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
            playerAnimatorController.PlayJump();
            GCamarada.AudioManager.I?.OnShotClip("Jump");
            StartJump();
        }
        else
        {
            if (hasJumped && !mushroomBounceActive &&
            PlayerInput.Instance.jumpAction.WasReleasedThisFrame() && verticalVelocity > 0f)
            {
                verticalVelocity *= 0.5f;
            }
        }
    }

    private void StartJump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        hasJumped = true;
        lastJumpTime = Time.time;
    }

    public void Launch() => StartJump();

    public void BounceFromMushroom(float height)
    {
        if (height <= 0f || !characterController.enabled)
            return;

        verticalVelocity = Mathf.Sqrt(
            2f * Mathf.Abs(gravity) * height
        );

        hasJumped = true;
        mushroomBounceActive = true;

        lastJumpTime = Time.time;
        lastGroundedTime = -10f;

        isGrounded = false;
        isSliding = false;
    }

    private void ApplyGravity()
    {
        if(isGrounded && verticalVelocity <= 0f)
        {
            verticalVelocity = -2f;
            return;
        }

        verticalVelocity += gravity * Time.deltaTime;

        if(verticalVelocity <= maxGravityVelocity)
        {
            verticalVelocity = maxGravityVelocity;
        }
    }

    

    private bool CanJump()
    {
        return !hasJumped && (isGrounded || Time.time - lastGroundedTime <= coyoteTime);
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        isSliding = false;
        groundNormal = Vector3.up;
        groundAngle = 0f;

        if (verticalVelocity > 0.01f || Time.time < lastJumpTime + ignoreGroundAfterJump)
            return;

        if (!ProbeGround(groundCheckDistance, false, out RaycastHit hit, out Vector3 normal))
            return;

        groundNormal = normal;
        groundAngle = Vector3.Angle(normal, Vector3.up);

        if(groundAngle <= characterController.slopeLimit)
        {
            isGrounded = true;
            hasJumped = false;
            mushroomBounceActive = false;
            lastGroundedTime = Time.time;
        }
        else
        {
            isSliding = true;
        }
    }

    private bool ProbeGround(float extraDistance, bool isSnapProbe, out RaycastHit hit, out Vector3 normal)
    {
        const float lift = 0.1f;
        float radius = characterController.radius - groundRadiusShrink;

        Vector3 center = transform.TransformPoint(characterController.center);
        Vector3 origin = center
            + Vector3.down * (characterController.height * 0.5f - characterController.radius)
            + Vector3.up * lift;

        float distance = lift + groundRadiusShrink + extraDistance;

        normal = Vector3.up;
        bool found  = Physics.SphereCast(origin, radius, Vector3.down, out hit, distance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (found)
        {
            normal = hit.normal;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit rayHit, distance + radius, groundLayerMask, QueryTriggerInteraction.Ignore))
                normal = rayHit.normal;
        }

#if UNITY_EDITOR
        ProbeDebug dbg = new ProbeDebug
        {
            valid = true,
            origin = origin,
            radius = radius,
            distance = distance,
            hit = found,
            hitPoint = found ? hit.point : Vector3.zero,
            hitNormal = normal,
            sphereCenter = found ? origin + Vector3.down * hit.distance : Vector3.zero
        };

        if (isSnapProbe) debugSnap = dbg;
        else debugGround = dbg;
#endif

        return found;
    }

#if UNITY_EDITOR
    [Header("Debug Data")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private float vectorGizmoScale = 0.2f;

    private struct ProbeDebug
    {
        public bool valid;
        public Vector3 origin;
        public float radius;
        public float distance;
        public bool hit;
        public Vector3 hitPoint;
        public Vector3 hitNormal;     // normal do raio central (a usada na lógica)
        public Vector3 sphereCenter;  // onde a esfera parou (ou fim do cast se não acertou)
    }

    private ProbeDebug debugGround;   // sonda do CheckGrounded
    private ProbeDebug debugSnap;     // sonda do SnapToGround (só preenchida quando snap roda)
    private Vector3 debugFinalVelocity;

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !Application.isPlaying || characterController == null)
            return;

        DrawProbe(debugGround, Color.cyan);
        DrawProbe(debugSnap, Color.magenta);

        // Estado do chão logo acima da cabeça
        Vector3 top = transform.position + Vector3.up * (characterController.height + 0.3f);
        Gizmos.color = isGrounded ? Color.green : (isSliding ? Color.yellow : Color.red);
        Gizmos.DrawSphere(top, 0.08f);

        // Vetor de velocidade final
        Vector3 origin = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + debugFinalVelocity * vectorGizmoScale);
        Gizmos.DrawSphere(origin + debugFinalVelocity * vectorGizmoScale, 0.05f);
    }

    private void DrawProbe(ProbeDebug p, Color color)
    {
        if (!p.valid) return;

        Vector3 end = p.origin + Vector3.down * p.distance;

        Gizmos.color = color;
        Gizmos.DrawWireSphere(p.origin, p.radius);   // início do cast
        Gizmos.DrawLine(p.origin, end);              // trajeto
        Gizmos.DrawWireSphere(end, p.radius);        // fim máximo do cast

        if (!p.hit) return;

        // Esfera na posição em que realmente parou
        Gizmos.color = new Color(color.r, color.g, color.b, 0.5f);
        Gizmos.DrawWireSphere(p.sphereCenter, p.radius);

        // Ponto de contato + normal
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(p.hitPoint, 0.035f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p.hitPoint, p.hitPoint + p.hitNormal * 0.3f);
    }
#endif

    public void TeleportTo(Transform teleportPoint)
    {
        // Desliga o CharacterController
        characterController.enabled = false;

        // Zera todo movimento anterior
        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;
        mushroomBounceActive = false;

        // Teleporta exatamente o PlayerCharacter
        transform.position = teleportPoint.position;
        transform.rotation = teleportPoint.rotation;

        // Liga novamente
        characterController.enabled = true;
    }
    private void HandleFootsteps()
    {
        if (isGrounded && horizontalVelocity.sqrMagnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                GCamarada.AudioManager.I?.PlaySequentialClip("GrassFootstep");
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; 
        }
    }
}