using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TimeTravel : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputAction resetAction;
    private InputAction travelAction;

    [Header("Layers")]
    private int playerLayer;
    private int pastLayer;
    private int futureLayer;

    [Header("Player")]
    [SerializeField] private bool isTravelling = false;
    [SerializeField] private float travelDuration = 3f;
    private float travelDurationTimer;

    [SerializeField] private float cooldown = 5f;
    private float cooldownTimer;
    private bool onCooldown = false;

    [Header("Cameras")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera pastCamera;
    
    [Header("Time Shader")]
    [SerializeField] private Material timeTravelMaterial;
    [SerializeField] private Transform timeDomeTransform;

    [Header("Time Dome")]
    [SerializeField] private MeshRenderer timeDome;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player");
        resetAction = inputActions.FindAction("Reset");
        travelAction = inputActions.FindAction("Interact");

        playerLayer = LayerMask.NameToLayer("Player");
        futureLayer = LayerMask.NameToLayer("Future");
        pastLayer = LayerMask.NameToLayer("Past");

        travelDurationTimer = travelDuration;
        cooldownTimer = cooldown;
    }

    private void Start()
    {
        // Estado físico inicial: FUTURO
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);

        // Main Camera nunca renderiza o passado.
        SetMainCameraToFuture();

        // Past Camera só será usada durante a habilidade.
        pastCamera.enabled = false;
        
        // Dome começa desligado.
        //timeDome.enabled = false;


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
        HandleReset();
        HandleTravel();
        UpdateTimeShader();
    }

    private void HandleReset()
    {
        if (resetAction.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void HandleTravel()
    {
        // Cooldown
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                cooldownTimer = cooldown;
            }
        }

        // Duração da viagem
        if (isTravelling)
        {
            travelDurationTimer -= Time.deltaTime;

            if (travelDurationTimer <= 0f)
            {
                EndTravel();
            }
        }

        // Ativação manual
        if (travelAction.WasPressedThisFrame() && !isTravelling && !onCooldown)
        {
            StartTravel();
        }
    }

    private void StartTravel()
    {
        isTravelling = true;
        travelDurationTimer = travelDuration;

        Debug.Log("Viajei pro passado");

        // Física do passado
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, true);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, false);

        // Visual do passado
        pastCamera.enabled = true;
        //timeDome.enabled = true;
    }

    private void EndTravel()
    {
        isTravelling = false;

        Debug.Log("Voltei pro futuro");

        // Física do futuro
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);

        // Visual do passado desligado
        pastCamera.enabled = false;
        //timeDome.enabled = false;
       

        // Cooldown
        onCooldown = true;
        cooldownTimer = cooldown;
    }

    private void SetMainCameraToFuture()
    {
        int pastMask = 1 << pastLayer;

        playerCamera.cullingMask &= ~pastMask;
    }

    private void UpdateTimeShader()
    {
        if (timeTravelMaterial == null || timeDomeTransform == null)
            return;

        Vector3 center = timeDomeTransform.position;

        float radius = timeDomeTransform.lossyScale.x * 0.5f;

        timeTravelMaterial.SetVector("_SphereCenter", center);
        timeTravelMaterial.SetFloat("_SphereRadius", isTravelling ? radius : 0f);
    }
}