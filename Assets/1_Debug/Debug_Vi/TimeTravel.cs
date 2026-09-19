using Unity.VisualScripting;
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
    [SerializeField] private/*  const */ float travelDuration = 3;
    private float travelDurationTimer = 3;
    private bool onCooldown = false;
    [SerializeField] private/*  const */ float cooldown = 5;
    private float cooldownTimer = 3;
    private MeshRenderer timeDome;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player");
        resetAction = inputActions.FindAction("Reset");
        travelAction = inputActions.FindAction("Interact");
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
    }

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        futureLayer = LayerMask.NameToLayer("Future");
        pastLayer = LayerMask.NameToLayer("Past");
        Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
        Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        timeDome = GameObject.Find("TimeDome").GetComponent<MeshRenderer>();
        timeDome.enabled = false;
    }

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void Update()
    {
        HandleReset();
        HandleTravel();
    }

    private void HandleReset()  // Só pra fim de teste
    {
        if (resetAction.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void HandleTravel()
    {
        // Timer do cooldown
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            
            if (cooldownTimer <= 0)
            {
                onCooldown = false;
                cooldownTimer = cooldown;
            }
        }
        
        // Timer da duração da viagem
        if (isTravelling)
        {
            travelDurationTimer -= Time.deltaTime;

            if (travelDurationTimer <= 0)
            {
                Travel();
                travelDurationTimer = travelDuration;
            }
        }
        
        // Input da Viagem ou retorno
        if (travelAction.WasPressedThisFrame() && !onCooldown)
        {
            Travel();
        }
    }

    private void Travel()
    {
        // Viaja se não estiver em cooldown
        isTravelling = !isTravelling;
        if (onCooldown)
        {
            isTravelling = false;
        }
        
        // Viaja
        if (isTravelling)
        {
            Debug.Log("viajei pro passado");
            Physics.IgnoreLayerCollision(playerLayer, futureLayer, true);
            Physics.IgnoreLayerCollision(playerLayer, pastLayer, false);
            timeDome.enabled = true;
        }
        
        //Volta da viagem e ativa cooldown
        else
        {
            Debug.Log("voltei pro futuro");
            Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
            Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
            onCooldown = true;
            timeDome.enabled = false;
        }
    }
}
