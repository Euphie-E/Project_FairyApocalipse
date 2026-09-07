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

    [Header("Layer")]
    private int playerLayer;
    private int pastLayer;
    private int futureLayer;

    [Header("Player State")]
    [SerializeField] private bool travelling = false;

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
        if (travelAction.WasPressedThisFrame())
        {
            Travel();
        }
    }

    private void Travel()
    {
        travelling = !travelling;
        if (travelling)
        {
            Debug.Log("viajei pro passado");
            Physics.IgnoreLayerCollision(playerLayer, futureLayer, true);
            Physics.IgnoreLayerCollision(playerLayer, pastLayer, false);
        }
        else
        {
            Debug.Log("viajei pro futuro");
            Physics.IgnoreLayerCollision(playerLayer, futureLayer, false);
            Physics.IgnoreLayerCollision(playerLayer, pastLayer, true);
        }
    }
}
