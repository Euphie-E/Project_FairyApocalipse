using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

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
    public InputAction dashAction { get; private set; }

    public enum InputType
    {
        Xbox,
        PlayStation,
        Keyboard
    }


    // gambiarra
    public InputAction um { get; private set; }
    public InputAction dois { get; private set; }
    public InputAction tres { get; private set; }
    public InputAction quatro { get; private set; }
    public InputAction cinco { get; private set; }
    public InputAction test { get; private set; }
    Vector3[] tpPositions = new Vector3[4];


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
        dashAction = inputSystemActions.Player.Dash;

        // gambiarra
        um = inputSystemActions.Player.Previous;
        dois = inputSystemActions.Player.Next;
        tres = inputSystemActions.Player.treis;
        quatro = inputSystemActions.Player.cuatro;
        cinco = inputSystemActions.Player.cinco;
        test = inputSystemActions.Player.Test;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // gambiarra
    void Start()
    {
        um.performed += ctx => TeleportToCheckpoint(0);
        dois.performed += ctx => TeleportToCheckpoint(1);
        tres.performed += ctx => TeleportToCheckpoint(2);
        quatro.performed += ctx => TeleportToCheckpoint(3);

        cinco.performed += ctx =>
        {
            if (SceneManager.GetActiveScene().buildIndex == 1) SceneManager.LoadScene(2);
            else if (SceneManager.GetActiveScene().buildIndex == 2) SceneManager.LoadScene(1);
        };


        moveAction.performed += CheckControl;
    }
    //this should not be done but the idex starts at 1
    Dictionary<Vector3,int> tpSort = new();
    public void AddCheckpoint(GameObject position, int index)
    {
        tpSort.Add(position.transform.position + Vector3.up * 1.5f,index);
        List<KeyValuePair<Vector3,int>> valuePairs = new();
        foreach (var item in tpSort)
        {
            valuePairs.Add(item);
        }
        valuePairs.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        int i = 0;
        foreach (var item in valuePairs)
        {
            tpPositions[i] = item.Key;
            i++;
            if(i>=tpPositions.Length) break;
        }
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
        dashAction.Enable();

        //gambiarra

        um.Enable();
        dois.Enable();
        tres.Enable();
        quatro.Enable();
        cinco.Enable();
        test.Enable();
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
        dashAction.Disable();

        //gambiarra

        um.Disable();
        dois.Disable();
        tres.Disable();
        quatro.Disable();
        cinco.Disable();
        test.Disable();
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
        dashAction.Disable();
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
        dashAction.Enable();
        if (DataManager.Instance != null)
            DataManager.Instance.AddData(DataManager.Data.Death,1);
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
            moveAction.performed += action;
        }
        if(type == 2)
        {
            jumpAction.performed += action;
        }
        if(type == 3)
        {
            interactAction.performed += action;
        }
        if(type == 4)
        {
            resetAction.performed += action;
        }
        if(type == 5)
        {
            travelAction.performed += action;
        }
        if(type == 6)
        {
            escapeAction.performed += action;
        }
        if(type == 7)
        {
            test.performed += action;
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
            moveAction.canceled += action;
        }
        if(type == 2)
        {
            jumpAction.canceled += action;
        }
        if(type == 3)
        {
            interactAction.canceled += action;
        }
        if(type == 4)
        {
            resetAction.canceled += action;
        }
        if(type == 5)
        {
            travelAction.canceled += action;
        }
        if(type == 6)
        {
            escapeAction.canceled += action;
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
            moveAction.performed -= action;
        }
        if(type == 2)
        {
            jumpAction.performed -= action;
        }
        if(type == 3)
        {
            interactAction.performed -= action;
        }
        if(type == 4)
        {
            resetAction.performed -= action;
        }
        if(type == 5)
        {
            travelAction.performed -= action;
        }
        if(type == 6)
        {
            escapeAction.performed -= action;
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
            moveAction.canceled -= action;
        }
        if(type == 2)
        {
            jumpAction.canceled -= action;
        }
        if(type == 3)
        {
            interactAction.canceled -= action;
        }
        if(type == 4)
        {
            resetAction.canceled -= action;
        }
        if(type == 5)
        {
            travelAction.canceled -= action;
        }
        if(type == 6)
        {
            escapeAction.canceled -= action;
        }
    }

    List<Action<InputType>> KeyMappingList = new();
    public void KeyMapping(Action<InputType> action)
    {
        if(!KeyMappingList.Contains(action)) KeyMappingList.Add(action);
    }
    public void RemoveKeyMapping(Action<InputType> action)
    {
        KeyMappingList.Remove(action);
    }
    void CheckControl(InputAction.CallbackContext ctx)
    {
        InputType resp = InputType.Keyboard;
        if(ctx.control.device is Gamepad && Gamepad.current != null)
        {
            if(Gamepad.current is XInputController || Gamepad.current.layout.Contains("Xbox"))
            {
                resp = InputType.Xbox;
            }
            if(Gamepad.current is DualShockGamepad || Gamepad.current.layout.Contains("DualShock"))
            {
                resp = InputType.PlayStation;
            }
        } 
        foreach (var item in KeyMappingList)
        {
            item(resp);
        }
        Debug.Log(resp.ToString());
    }


    private void TeleportToCheckpoint(int index)
    {
        if (index < 0 || index >= tpPositions.Length)
            return;

        CharacterController characterController =
            GetComponent<CharacterController>();

        characterController.enabled = false;
        transform.position = tpPositions[index];
        characterController.enabled = true;

        PlayerDeathController deathController =
            GetComponent<PlayerDeathController>();

        if (deathController != null)
            deathController.ResetFallAfterTeleport();

        Checkpoint[] checkpoints =
            FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint != null &&
                Vector3.Distance(
                    checkpoint.transform.position + Vector3.up * 1.5f,
                    tpPositions[index]) < 0.1f)
            {
                checkpoint.PlayRespawnEffect();
                break;
            }
        }
    }
    }
