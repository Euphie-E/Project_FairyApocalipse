using System;
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


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        inputSystemActions ??= new InputSystem_Actions();
        playerMap = inputActions.FindActionMap("Player");
        moveAction = inputSystemActions.Player.Move;//playerMap.FindAction("Move");
        jumpAction = inputSystemActions.Player.Jump;//playerMap.FindAction("Jump");
        interactAction = inputSystemActions.Player.Interact;
        resetAction = inputSystemActions.Player.Reset;
    }

    private void OnEnable()
    {
        playerMap.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        resetAction.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
        moveAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        resetAction.Disable();
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
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
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
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
    }


    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
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
    }

    /// <summary>
    /// Type:<br />
    /// 1: Move<br />
    /// 2: Jump<br />
    /// 3: Interact<br />
    /// 4: Reset<br />
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
    }

    public void OnDeath()
    {
        playerMap.Disable();
        moveAction.Disable();
        jumpAction.Disable();
    }

    public void OnRevive()
    {
        playerMap.Disable();
        moveAction.Disable();
        jumpAction.Disable();
    }
}
