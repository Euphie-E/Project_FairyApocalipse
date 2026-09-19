using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TimeTravel : MonoBehaviour
{
    //[Header("Input")]
    //[SerializeField] private InputActionAsset inputActions;

    //private InputActionMap playerMap;
    //private InputAction resetAction;
    //private InputAction travelAction;

    [Header("Layers")]
    private int playerLayer;
    private int notPlayerLayer;
    private int pastLayer;
    private int futureLayer;

    [Header("Player")]
    [SerializeField] Transform notPlayerT;
    [SerializeField] private bool isTravelling = false;
    [SerializeField] private float travelDuration = 3f;
    private float travelDurationTimer;

    [SerializeField] private float cooldown = 5f;
    private float cooldownTimer;
    private bool onCooldown = false;

    [Header("Cameras")]
    [SerializeField] private Camera playerCamera = null;
    //[SerializeField] private Camera pastCamera;
    
    [Header("Time Shader")]
    [SerializeField] private Material timeTravelMaterial;
    [SerializeField] private Transform timeDomeTransform;

    [Header("Time Dome")]
    [SerializeField] private MeshRenderer timeDome;
    Collider[] colliders = new Collider[5];

    private void Awake()
    {
        //playerMap = inputActions.FindActionMap("Player");
        //resetAction = inputActions.FindAction("Reset");
        //travelAction = inputActions.FindAction("Interact");

        playerLayer = LayerMask.NameToLayer("Player");
        //notPlayerLayer = LayerMask.NameToLayer("Playernt");
        futureLayer = LayerMask.NameToLayer("Future");
        pastLayer = LayerMask.NameToLayer("Past");

        travelDurationTimer = travelDuration;
        cooldownTimer = cooldown;
        playerCamera ??= Camera.main;
    }

    private void Start()
    {
        // Estado físico inicial: FUTURO
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        //Physics.IgnoreLayerCollision(notPlayerLayer, futureLayer, true);
        //Physics.IgnoreLayerCollision(notPlayerLayer, pastLayer, false);
        //Physics.IgnoreLayerCollision(notPlayerLayer, playerLayer, true);
        //Physics.IgnoreLayerCollision(playerLayer, notPlayerLayer, true);

        // Main Camera nunca renderiza o passado.
        //SetMainCameraToFuture();

        // Past Camera só será usada durante a habilidade.
        //pastCamera.enabled = false;
        
        // Dome começa desligado.
        //timeDome.enabled = false;

        PlayerInput.Instance.AddAction(Reset,4);
        PlayerInput.Instance.AddAction(StartTravel,3); 

    }

    private void OnEnable()
    {
        //playerMap.Enable();
    }

    private void OnDisable()
    {
        //playerMap.Disable();
    }

    private void Update()
    {
        //HandleReset();
        HandleTravel();
        UpdateTimeShader();
        //Debug.DrawRay(notPlayerT.position+Vector3.up*up,Vector3.down*z,Color.red);
    }

    /* private void HandleReset()
    {
        if (resetAction.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    } */

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
        /* if (travelAction.WasPressedThisFrame() && !isTravelling && !onCooldown)
        {
            StartTravel();
        } */
    }

    private void StartTravel()
    {
        if(isTravelling || onCooldown) return;
        if(CheckNotPlayer())
        {
            Debug.Log("Vou entar em algo");
            return;
        }
        isTravelling = true;
        travelDurationTimer = travelDuration;

        Debug.Log("Viajei pro passado");

        // Física do passado
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, true);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, false);
        //Physics.IgnoreLayerCollision(notPlayerLayer, futureLayer, false);
        //Physics.IgnoreLayerCollision(notPlayerLayer, pastLayer, true);

        // Visual do passado
        //Debug.Log(playerCamera.cullingMask);
        //pastCamera.enabled = true;
        //timeDome.enabled = true;
        SetMainCameraToPast();
    }

    private void EndTravel()
    {
        if(CheckNotPlayer())
        {
            Debug.Log("Vou entar em algo");
            return;
        }
        isTravelling = false;

        Debug.Log("Voltei pro futuro");

        // Física do futuro
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        //Physics.IgnoreLayerCollision(notPlayerLayer, futureLayer, true);
        //Physics.IgnoreLayerCollision(notPlayerLayer, pastLayer, false);

        // Visual do passado desligado
        //pastCamera.enabled = false;
        //timeDome.enabled = false;
        SetMainCameraToFuture();
       

        // Cooldown
        onCooldown = true;
        cooldownTimer = cooldown;
    }

    private void SetMainCameraToFuture()
    {
        int pastMask = 1 << pastLayer;
        int futureMask = 1 << futureLayer;

        playerCamera.cullingMask ^= ~pastMask;
        playerCamera.cullingMask ^= ~futureMask;
    }

    private void SetMainCameraToPast()
    {
        int futureMask = 1 << futureLayer;
        int pastMask = 1 << pastLayer;

        playerCamera.cullingMask ^= ~futureMask;
        playerCamera.cullingMask ^= ~pastMask;
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
    bool CheckNotPlayer()
    {
        //Debug.Log(Physics.OverlapSphereNonAlloc(notPlayerT.position,10,colliders,notPlayerLayer));
        //if(Physics.OverlapSphereNonAlloc(notPlayerT.position,10,colliders,notPlayerLayer) > 1)
        //{
        //    return true;
        //}
        //if(colliders[0].CompareTag("Player"))return false;
        //return true;

        
        return false;//Physics.Raycast(notPlayerT.position+Vector3.up*up,Vector3.down,z,notPlayerLayer);
    }

    void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}