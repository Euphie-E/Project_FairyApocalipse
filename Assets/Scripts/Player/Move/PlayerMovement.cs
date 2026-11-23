using GCamarada;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour, IMushroomBounceReceiver
{
    [Header("Footsteps Audio")]
    [SerializeField] private float stepInterval = 0.4f;

    [Header("Footsteps VFX")]
    public VisualEffect footstepsVfx;

    [Header("Landing VFX")]
    [SerializeField] private VisualEffect landingVFX;
    private bool wasGrounded;

    private float stepTimer;
    [Header("References")]
    public Transform cameraTransform;
    private PlayerAnimatorController playerAnimatorController;
    public CharacterController characterController { get; private set; }
    private TimeTravel timeTravel;
    PlayerDeathController deathController;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float uprightSpeed = 360f;
    private Vector3 lastFacingDirection = Vector3.forward;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float maxGravityVelocity = -10f;
    public Vector3 horizontalVelocity { get; private set; }
    public float verticalVelocity { get; private set; }

    private bool mushroomBounceActive;

    [Header("Ground Check")]
    public bool animatorGrounded;
    public float groundedDelay = 0.15f;
    private float airborneTimer;
    private Vector3 steepNormal = Vector3.up;
    private float lastSteepContactTime = -10f;
    private const float steepContactMemory = 0.1f;
    [SerializeField] private float ledgePushSpeed = 2.5f;
    private LayerMask groundLayerMask;
    private Vector3 smoothedNormal = Vector3.up;
    [SerializeField] LayerMask groundLayerMaskPast;
    [SerializeField] LayerMask groundLayerMaskFuture;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float snapDistance = 0.4f;
    [SerializeField] private float groundRadiusShrink = 0.05f;
    private Vector3 groundNormal = Vector3.up;
    [SerializeField] private float normalSmoothing = 20f;
    private float groundAngle;
    public bool isGrounded { get; private set; }
    public bool isSliding { get; private set; }

    [Header("Coyote Jump")]
    [SerializeField] private float coyoteTime = 0.12f;
    private float lastGroundedTime = -10f;
    private float lastJumpTime = -10f;
    private const float ignoreGroundAfterJump = 0.1f;
    private bool hasJumped;

    [Header("Dash")]
    public bool dashAvailable = true;
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 1f;
    private float dashCooldownTimer;
    public bool isDashing { get; private set; }
    private bool canDash = true;
    private Vector3 dashDirection;
    private float dashTraveled;
    private float DashSpeed => dashDistance / dashDuration;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private void OnEnable()
    {
        canDash = true;
    }

    private void OnDisable()
    {
        isDashing = false;        
    }

    private void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        characterController = GetComponent<CharacterController>();
        timeTravel = GetComponent<TimeTravel>();
        deathController = GetComponent<PlayerDeathController>();
    }

    private void Start()
    {
        wasGrounded = true;
        cameraTransform = Camera.main.transform;
        FutureCollision();

        if (SceneManager.GetActiveScene().name == "01Spring")
            DisableDash();
    }

    private void Update()
    {
        if (!characterController.enabled) return;

        if (isDashing)
        {
            UpdateDash();
            return;
        }

        UpdateDashStatus();
        CheckGrounded();
        UpdateAnimatorGrounded();
        HandleLandingVFX();

        if (StartDash())
        {
            UpdateDash();
            return;
        }

        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleFootsteps();

        Vector3 velocity = CalculateFinalVelocity();
        velocity = ApplySteepContact(velocity);
        bool groundedBeforeMove = isGrounded;

        CollisionFlags flags = characterController.Move(velocity * Time.deltaTime);

        if (!isGrounded && Time.time - lastSteepContactTime <= steepContactMemory)
        {
            Vector3 nH = new Vector3(steepNormal.x, 0f, steepNormal.z);
            if (nH.sqrMagnitude > 0.0001f)
            {
                nH.Normalize();
                float into = Vector3.Dot(horizontalVelocity, nH);
                if (into < 0f)
                    horizontalVelocity -= nH * into;
                {

                }
            }
        }

        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f) //Bateu no teto
            verticalVelocity *= 0.5f;

        if ((flags & CollisionFlags.Sides) != 0)
        {
            Vector3 actual = characterController.velocity; actual.y = 0f;
            if (actual.sqrMagnitude < horizontalVelocity.sqrMagnitude)
                horizontalVelocity = actual;
        }

        if (groundedBeforeMove && !characterController.isGrounded)
            SnapToGround();

#if UNITY_EDITOR
        debugFinalVelocity = velocity;
#endif
    }

    private Vector3 ApplySteepContact(Vector3 velocity)
    {
        if (isGrounded) return velocity;
        if (Time.time - lastSteepContactTime > steepContactMemory) return velocity;

        float into = Vector3.Dot(velocity, steepNormal);
        if (into < 0f)
            velocity -= steepNormal * into;

        return velocity;
    }

    private Vector3 CalculateFinalVelocity()
    {
        if (isGrounded && verticalVelocity <= 0f)
        {
            Vector3 v = horizontalVelocity;
            if (groundNormal.y > 0.01f)
                v.y = -(groundNormal.x * v.x + groundNormal.z * v.z) / groundNormal.y;

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

        if (ProbeGround(snapDistance, true, out RaycastHit hit, out Vector3 normal, out bool axisSuported) && axisSuported)
        {
            float angle = Vector3.Angle(normal, Vector3.up);
            float drop = hit.distance - 0.1f - groundRadiusShrink;
            bool isSlope = angle > 1f;

            if (angle <= characterController.slopeLimit && drop > 0.001f && (isSlope || drop <= characterController.stepOffset))
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
        if (moveDirection.sqrMagnitude > 1f)
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
        else
        {
            RecoverUpright();
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

    private void RecoverUpright()
    {
        if (Vector3.Angle(transform.up, Vector3.up) < 0.1f) return;

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = lastFacingDirection;

        Quaternion upright = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, upright, uprightSpeed * Time.deltaTime);
    }

    private void HandleLandingVFX()
    {
        if (isGrounded && !wasGrounded)
        {
            if (landingVFX != null)
                landingVFX.Play();
        }

        wasGrounded = isGrounded;
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

        animatorGrounded = false;
        airborneTimer = groundedDelay;
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

        animatorGrounded = false;
        airborneTimer = groundedDelay;
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalVelocity <= 0f)
        {
            verticalVelocity = -2f;
            return;
        }

        verticalVelocity += gravity * Time.deltaTime;

        if (verticalVelocity <= maxGravityVelocity)
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
        bool wasGrounded = isGrounded || isSliding;

        isGrounded = false;
        isSliding = false;
        groundNormal = Vector3.up;
        groundAngle = 0f;

        if (verticalVelocity > 0.01f || Time.time < lastJumpTime + ignoreGroundAfterJump)
            return;

        if (!ProbeGround(groundCheckDistance, false, out RaycastHit hit, out Vector3 normal))
            return;

        groundNormal = normal;
        smoothedNormal = wasGrounded
            ? Vector3.Slerp(smoothedNormal, normal, 1f - Mathf.Exp(-normalSmoothing * Time.deltaTime)).normalized
            : normal;
        groundNormal = smoothedNormal;
        groundAngle = Vector3.Angle(normal, Vector3.up);

        if (groundAngle <= characterController.slopeLimit)
        {
            isGrounded = true;
            hasJumped = false;
            lastGroundedTime = Time.time;
        }
        else
        {
            isSliding = true;
        }
    }

    void UpdateAnimatorGrounded()
    {

        if (isGrounded)
        {
            airborneTimer = 0f;
            animatorGrounded = true;
        }
        else
        {
            airborneTimer += Time.deltaTime;

            if (airborneTimer >= groundedDelay)
                animatorGrounded = false;
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
        bool found = Physics.SphereCast(origin, radius, Vector3.down, out hit, distance, groundLayerMask, QueryTriggerInteraction.Ignore);

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

    private void UpdateDashStatus()
    {
        if (!canDash)
        {
            dashCooldownTimer += Time.deltaTime;

            if (isGrounded && dashCooldownTimer >= dashCooldown)
            {
                canDash = true;
                dashCooldownTimer = 0f;
            }  
        }
    }

    private bool StartDash()
    {
        if (!dashAvailable) return false;
        if (!canDash) return false;
        if (!PlayerInput.Instance.dashAction.WasPressedThisFrame()) return false;

        Vector2 input = PlayerInput.Instance.moveAction.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 dir = right * input.x + forward * input.y;

        if (dir.sqrMagnitude < 0.001f)
            dir = transform.forward;

        dashDirection = dir.normalized;
        dashTraveled = 0f;
        isDashing = true;
        canDash = false;
        dashCooldownTimer = 0f;

        lastGroundedTime = -10f;
        verticalVelocity = 0f;

        transform.rotation = Quaternion.LookRotation(dashDirection);

        playerAnimatorController.PlayDash();
        //Trigger audio

        return true;
    }

    private void UpdateDash()
    {
        float step = Mathf.Min(DashSpeed * Time.deltaTime, dashDistance - dashTraveled);

        Vector3 before = transform.position;
        CollisionFlags flags = characterController.Move(dashDirection * step);
        dashTraveled += Vector3.Distance(before, transform.position);

        bool reachedDistance = dashTraveled >= dashDistance - 0.001f;
        bool hitWall = (flags & CollisionFlags.Sides) != 0;

        if (reachedDistance || hitWall)
            EndDash();
    }

    private void EndDash()
    {
        isDashing = false;

        horizontalVelocity = dashDirection * maxSpeed;
        verticalVelocity = 0f;
    }

    private bool ProbeGround(float extraDistance, bool isSnapProbe, out RaycastHit hit, out Vector3 normal, out bool axisSuported)
    {
        const float lift = 0.1f;
        float radius = characterController.radius - groundRadiusShrink;

        Vector3 center = transform.TransformPoint(characterController.center);
        Vector3 origin = center
            + Vector3.down * (characterController.height * 0.5f - characterController.radius)
            + Vector3.up * lift;

        float distance = lift + groundRadiusShrink + extraDistance;

        normal = Vector3.up;
        axisSuported = false;
        bool found = Physics.SphereCast(origin, radius, Vector3.down, out hit, distance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if (found)
        {
            normal = hit.normal;
            if(Physics.Raycast(origin, Vector3.down, out RaycastHit rayHit, distance + radius, groundLayerMask, QueryTriggerInteraction.Ignore))
            {
                normal = rayHit.normal;
                axisSuported = true;
            }
            else
            {
                Vector3 toPlataform = Vector3.ProjectOnPlane(hit.point - origin, Vector3.up).normalized;
                Vector3 start = hit.point + toPlataform * 0.02f + Vector3.up * 0.1f;
                if (Physics.Raycast(start, Vector3.down, out RaycastHit faceHit, 0.3f, groundLayerMask, QueryTriggerInteraction.Ignore))
                    normal = faceHit.normal;
            }
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
        public Vector3 hitNormal;     // normal do raio central (a usada na l�gica)
        public Vector3 sphereCenter;  // onde a esfera parou (ou fim do cast se n�o acertou)
    }

    private ProbeDebug debugGround;   // sonda do CheckGrounded
    private ProbeDebug debugSnap;     // sonda do SnapToGround (s� preenchida quando snap roda)
    private Vector3 debugFinalVelocity;

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !Application.isPlaying || characterController == null)
            return;

        DrawProbe(debugGround, Color.cyan);
        DrawProbe(debugSnap, Color.magenta);

        // Estado do ch�o logo acima da cabe�a
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
        Gizmos.DrawWireSphere(p.origin, p.radius);   // in�cio do cast
        Gizmos.DrawLine(p.origin, end);              // trajeto
        Gizmos.DrawWireSphere(end, p.radius);        // fim m�ximo do cast

        if (!p.hit) return;

        // Esfera na posi��o em que realmente parou
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

        Vector3 difference = teleportPoint.position - transform.position;

        // Teleporta exatamente o PlayerCharacter
        transform.SetPositionAndRotation(teleportPoint.position, teleportPoint.rotation);

        // Liga novamente
        characterController.enabled = true;
        
        cinemachineCamera.OnTargetObjectWarped(
            cinemachineCamera.Target.TrackingTarget,
            difference
        );
        ResetMotionForDeath();
        deathController.ResetFallAfterTeleport();
    }
    public void TeleportToCheckpoint(int index)
    {
        Checkpoint checkpoint = LevelManager.Get()?.GetCheckpoint(index);
        if(checkpoint == null)
        {
            Debug.Log("Checkpoint nao encontrado");
            return;
        }
        TeleportTo(checkpoint.transform);
        if (checkpoint != null)
        {
            checkpoint.PlayRespawnEffect();
        }
    }
    public void TeleportLastCheckpoint()
    {
        if(LevelManager.Get() == null) return;
        TeleportToCheckpoint(LevelManager.Get().GetLastCheckpoint().index);
    }
    private void HandleFootsteps()
    {
        if (animatorGrounded && horizontalVelocity.sqrMagnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                GCamarada.AudioManager.I?.PlaySequentialClip("GrassFootstep");

                if (footstepsVfx != null)
                {
                    footstepsVfx.Play();
                }

                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void PastCollision()
    {
        groundLayerMask = groundLayerMaskPast;
    }

    public void FutureCollision()
    {
        groundLayerMask = groundLayerMaskFuture;
    }

    public void ResetMotionForDeath()
    {
        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;

        hasJumped = false;
        mushroomBounceActive = false;

        isGrounded = false;
        isSliding = false;
        isDashing = false;
    }

    public void EnableDash()
    {
        dashAvailable = true;
    }

    public void DisableDash()
    {
        dashAvailable = false;
    }
}