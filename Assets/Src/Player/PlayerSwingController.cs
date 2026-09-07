using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingController : MonoBehaviour
{
    public bool isSwinging {  get; private set; }

    private Transform swingPivot;
    private SwingBarController currentBar;
    private Transform cameraTransform;

    [Header("Swing")]
    [SerializeField] private float inputForce = 120f;
    [SerializeField] private float gravityForce = 300f;
    [SerializeField] private float angularDrag = 2f;
    [SerializeField] private float maxAngularVelocity = 180f;
    [SerializeField] private float initialMultiplier = 20f;
    public float initialForwardSpeed = 0f;


    [Header("Angle")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;
    private float angle;
    private float angularVelocity;

    private void Awake()
    {
        isSwinging = false;
        enabled = false;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (swingPivot == null)
            return;

        //Le Input
        Vector2 input = PlayerInput.Instance.moveAction.ReadValue<Vector2>();

        //Pega a direção da camera e passa pra direção do input
        float cameraDirection = GetCameraDirection();
        input.y *= cameraDirection;

        ApplyInputForce(input);

        ApplyGravity();

        //Limita a velocidade
        angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);

        //Drag
        angularVelocity = Mathf.Lerp(angularVelocity, 0f, angularDrag * Time.deltaTime);

        angle += angularVelocity * Time.deltaTime;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        if (angle <= minAngle || angle >= maxAngle)
            angularVelocity = 0f;

        UpdateSwingRotation();
        Jump();
    }

    private void OnEnable()
    {
        isSwinging = true;

        angularVelocity = -initialForwardSpeed * initialMultiplier;
        if (swingPivot != null)
            angle = 0f;
    }

    private void OnDisable()
    {
        isSwinging = false;
    }

    public void SetCurrentBar(Transform swing, SwingBarController _currentBar)
    {
        swingPivot = swing;
        currentBar = _currentBar;
    }

    private void ApplyInputForce(Vector2 input)
    {
        if (Mathf.Abs(input.y) < 0.1f)
            return;

        angularVelocity += input.y * inputForce * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        float gravity = Mathf.Sin(angle * Mathf.Deg2Rad);

        angularVelocity -= gravity * gravityForce * Time.deltaTime;
    }

    private void UpdateSwingRotation()
    {
        swingPivot.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void Jump()
    {
        if (PlayerInput.Instance.jumpAction.WasPressedThisFrame())
        {
            Debug.Log("Sem angulo para se soltar");
            if (angle >= 45f || angle <= -45f)
            {
                transform.SetParent(null);
                isSwinging = false;
                currentBar.ResetSwingBar();

                PlayerStateController.Instance.EndSwing();
            }
        }
    }

    private float GetCameraDirection()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 barForward = transform.parent.forward;

        cameraForward.y = 0f;
        barForward.y = 0f;

        cameraForward.Normalize();
        barForward.Normalize();

        float dot = Vector3.Dot(cameraForward, barForward);
        return dot >= 0f ? -1f : 1f;
    }
}

