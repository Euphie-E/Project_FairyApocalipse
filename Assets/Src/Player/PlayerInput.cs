using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    InputSystem_Actions inputSystemActions;

    public InputActionMap playerMap { get; private set; }
    public InputAction moveAction { get; private set; }
    public InputAction jumpAction { get; private set; }
    public InputAction interactAction { get; private set; }
    public InputAction resetAction { get; private set; }
    public InputAction travelAction { get; private set; }
    public InputAction escapeAction { get; private set; }


    // gambiarra
    public InputAction um { get; private set; }
    public InputAction dois { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        playerMap = inputActions.FindActionMap("Player");
        inputSystemActions = new InputSystem_Actions();
        moveAction = inputSystemActions.Player.Move;//playerMap.FindAction("Move");
        jumpAction = inputSystemActions.Player.Jump;//playerMap.FindAction("Jump");
        interactAction = inputSystemActions.Player.Interact;
        resetAction = inputSystemActions.Player.Reset;
        travelAction = inputSystemActions.Player.Travel;
        escapeAction = inputSystemActions.Player.Escape;

        // gambiarra
        um = inputSystemActions.Player.Previous;
        dois = inputSystemActions.Player.Next;
    }

    // gambiarra
    void Start()
    {
        um.performed += ctx =>
        {
            transform.GetComponent<PlayerMovement>().enabled = false;
            transform.GetComponent<PlayerDeathController>().enabled = false;
            transform.localPosition = new Vector3(14,10.1f,60);
            StartCoroutine("Gambi");
        };
        dois.performed += ctx =>
        {
            transform.GetComponent<PlayerMovement>().enabled = false;
            transform.GetComponent<PlayerDeathController>().enabled = false;
            transform.localPosition = new Vector3(76.5f,-38.8f,147);
            StartCoroutine("Gambi");
        };
    }
    IEnumerator Gambi()
    {
        yield return new WaitForSeconds(0.5f);
        transform.GetComponent<PlayerMovement>().enabled = true;
        transform.GetComponent<PlayerDeathController>().enabled = true;
    }
    

    private void OnEnable()
    {
        playerMap.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        resetAction.Enable();
        travelAction.Enable();
        escapeAction.Enable();

        //gambiarra

        um.Enable();
        dois.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
        moveAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        resetAction.Disable();
        travelAction.Disable();
        escapeAction.Disable();

        //gambiarra

        um.Disable();
        dois.Disable();
    }

    public void OnDeath()
    {
        playerMap.Disable();
        moveAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        resetAction.Disable();
        travelAction.Disable();
        escapeAction.Disable();
    }

    public void OnRevive()
    {
        playerMap.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        resetAction.Enable();
        travelAction.Enable();
        escapeAction.Enable();
    }
    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void AddAction(Action<InputAction.CallbackContext> action,int type)
    {
        if(type == 1)
        {
            moveAction.performed += ctx => action(ctx);
        }
        if(type == 2)
        {
            jumpAction.performed += ctx => action(ctx);
        }
        if(type == 3)
        {
            interactAction.performed += ctx => action(ctx);
        }
        if(type == 4)
        {
            resetAction.performed += ctx => action(ctx);
        }
        if(type == 5)
        {
            travelAction.performed += ctx => action(ctx);
        }
        if(type == 6)
        {
            escapeAction.performed += ctx => action(ctx);
        }
        
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void AddAction(Action action,int type)
    {
        if(type == 1)
        {
            moveAction.performed += ctx => action();
        }
        if(type == 2)
        {
            jumpAction.performed += ctx => action();
        }
        if(type == 3)
        {
            interactAction.performed += ctx => action();
        }
        if(type == 4)
        {
            resetAction.performed += ctx => action();
        }
        if(type == 5)
        {
            travelAction.performed += ctx => action();
        }
        if(type == 6)
        {
            escapeAction.performed += ctx => action();
        }
    }


    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void AddCancelAction(Action<InputAction.CallbackContext> action,int type)
    {
        if(type == 1)
        {
            moveAction.canceled += ctx => action(ctx);
        }
        if(type == 2)
        {
            jumpAction.canceled += ctx => action(ctx);
        }
        if(type == 3)
        {
            interactAction.canceled += ctx => action(ctx);
        }
        if(type == 4)
        {
            resetAction.canceled += ctx => action(ctx);
        }
        if(type == 5)
        {
            travelAction.canceled += ctx => action(ctx);
        }
        if(type == 6)
        {
            escapeAction.canceled += ctx => action(ctx);
        }
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void AddCancelAction(Action action,int type)
    {
        if(type == 1)
        {
            moveAction.canceled += ctx => action();
        }
        if(type == 2)
        {
            jumpAction.canceled += ctx => action();
        }
        if(type == 3)
        {
            interactAction.canceled += ctx => action();
        }
        if(type == 4)
        {
            resetAction.canceled += ctx => action();
        }
        if(type == 5)
        {
            travelAction.canceled += ctx => action();
        }
        if(type == 6)
        {
            escapeAction.canceled += ctx => action();
        }
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void RemoveAction(Action<InputAction.CallbackContext> action,int type)
    {
        if(type == 1)
        {
            moveAction.performed += ctx => action(ctx);
        }
        if(type == 2)
        {
            jumpAction.performed += ctx => action(ctx);
        }
        if(type == 3)
        {
            interactAction.performed += ctx => action(ctx);
        }
        if(type == 4)
        {
            resetAction.performed += ctx => action(ctx);
        }
        if(type == 5)
        {
            travelAction.performed += ctx => action(ctx);
        }
        if(type == 6)
        {
            escapeAction.performed += ctx => action(ctx);
        }
        
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void RemoveAction(Action action,int type)
    {
        if(type == 1)
        {
            moveAction.performed -= ctx => action();
        }
        if(type == 2)
        {
            jumpAction.performed -= ctx => action();
        }
        if(type == 3)
        {
            interactAction.performed -= ctx => action();
        }
        if(type == 4)
        {
            resetAction.performed -= ctx => action();
        }
        if(type == 5)
        {
            travelAction.performed -= ctx => action();
        }
        if(type == 6)
        {
            escapeAction.performed -= ctx => action();
        }
    }


    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void RemoveCancelAction(Action<InputAction.CallbackContext> action,int type)
    {
        if(type == 1)
        {
            moveAction.canceled += ctx => action(ctx);
        }
        if(type == 2)
        {
            jumpAction.canceled += ctx => action(ctx);
        }
        if(type == 3)
        {
            interactAction.canceled += ctx => action(ctx);
        }
        if(type == 4)
        {
            resetAction.canceled += ctx => action(ctx);
        }
        if(type == 5)
        {
            travelAction.canceled += ctx => action(ctx);
        }
        if(type == 6)
        {
            escapeAction.canceled += ctx => action(ctx);
        }
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
    /// 5: Travel<br />
    /// 6: Esc<br />
    /// </summary>
    /// <param name="action"></param>
    /// <param name="type"></param>
    public void RemoveCancelAction(Action action,int type)
    {
        if(type == 1)
        {
            moveAction.canceled += ctx => action();
        }
        if(type == 2)
        {
            jumpAction.canceled += ctx => action();
        }
        if(type == 3)
        {
            interactAction.canceled += ctx => action();
        }
        if(type == 4)
        {
            resetAction.canceled += ctx => action();
        }
        if(type == 5)
        {
            travelAction.canceled += ctx => action();
        }
        if(type == 6)
        {
            escapeAction.canceled += ctx => action();
        }
    }
}
