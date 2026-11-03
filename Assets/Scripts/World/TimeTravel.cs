using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TimeTravel : MonoBehaviour
{
    /* [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputAction resetAction;
    private InputAction travelAction; */

    [Header("Layers")]
    private int playerLayer;
    public int pastLayer;
    public int futureLayer;
    int notPlayer;
    int realPlayer;
    [SerializeField] SphereCollider layerCheckBody;
    [SerializeField] SphereCollider layerCheckFeet;

    [Header("Player")]
    [SerializeField] public bool isTravelling = false;
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

    public bool TravelBlocked ;//{ get; private set; }
    PlayerSwingController psc;

    public void SetTravelBlocked(bool blocked)
    {
        TravelBlocked = blocked;

        // Entrou na area bloqueada estando no passado
        if (blocked && isTravelling)
        {
            EndTravel(true);
        }
    }

    private void Awake()
    {
        /* playerMap = inputActions.FindActionMap("Player");
        resetAction = inputActions.FindAction("Reset");
        travelAction = inputActions.FindAction("Travel"); */

        playerLayer = LayerMask.NameToLayer("Player");
        futureLayer = LayerMask.NameToLayer("Future");
        pastLayer = LayerMask.NameToLayer("Past");

        travelDurationTimer = travelDuration;
        cooldownTimer = cooldown;
    }

    private void Start()
    {
        // Inputs
        PlayerInput.Instance.AddAction(HandleReset,4);
        PlayerInput.Instance.AddAction(StartTravel,5);


        // Estado físico inicial: FUTURO
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        notPlayer = 1 << pastLayer;
        realPlayer = 1 << futureLayer;

        // Main Camera nunca renderiza o passado.
        SetMainCameraToFuture();

        // Past Camera só será usada durante a habilidade.
        pastCamera.enabled = false;
        
        // Dome começa desligado.
        //timeDome.enabled = false;

        psc = GetComponent<PlayerSwingController>();
    }

    /* private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    } */

    private void Update()
    {
        //HandleReset();
        HandleTravel();
        UpdateTimeShader();
    }

    public void HandleReset(InputAction.CallbackContext ctx)
    {
        //if (resetAction.WasPressedThisFrame())
        //{
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}
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
        //if (travelAction.WasPressedThisFrame() && !isTravelling && !onCooldown)
        //{
        //    StartTravel();
        //}
    }

    private void StartTravel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (onCooldown) return;
        if (!CheckTravel()) return;
        if (TravelBlocked)
            return;
        if (isTravelling) 
        {
            EndTravel();
            return;
        }
        isTravelling = true;
        travelDurationTimer = travelDuration;

        GCamarada.AudioManager.I?.PlayClip("StartTravel", true);
        Debug.Log("Viajei pro passado");

        // Física do passado
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, true);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, false);
        PlayerStateController.Instance.ActivatePastLayer();     
        notPlayer = 1 << futureLayer; 
        realPlayer = 1 << pastLayer;  

        // Visual do passado
        pastCamera.enabled = true;
        //timeDome.enabled = true;
        if (DataManager.Instance != null)
            DataManager.Instance.AddData(DataManager.Data.TimeTravel,1);
    }

    private void EndTravel(bool force = false)
    {
        if(!force && !CheckTravel()) return;
        isTravelling = false;

        GCamarada.AudioManager.I?.StopClip("StartTravel");
        Debug.Log("Voltei pro futuro");

        // Física do futuro
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        PlayerStateController.Instance.ActivateFutureLayer();
        notPlayer = 1 << pastLayer;
        realPlayer = 1 << futureLayer;

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

    bool CheckTravel()
    {
        if(psc.enabled) return false;
        bool notBody = layerCheckBody == null || Physics.CheckSphere(layerCheckBody.transform.position,layerCheckBody.radius,notPlayer);
        bool notFeet = layerCheckFeet == null || Physics.CheckSphere(layerCheckFeet.transform.position,layerCheckFeet.radius,notPlayer);
        bool feet = layerCheckFeet == null || Physics.CheckSphere(layerCheckFeet.transform.position,layerCheckFeet.radius,realPlayer);
        bool genericFeet = layerCheckFeet == null || Physics.CheckSphere(layerCheckFeet.transform.position,layerCheckFeet.radius,1 << 0);
        if (notBody) return false;
        if (genericFeet) return true;
        if (!notFeet && feet) return false;
        return true;
    }
}