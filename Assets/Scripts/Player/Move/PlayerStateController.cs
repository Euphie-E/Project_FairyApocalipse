using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public static PlayerStateController Instance;

    public PlayerMovement playerMovement { get; private set; }
    public PlayerSwingController playerSwingController { get; private set; }
    public PlayerCutsceneMovement playerCutsceneMovement { get; private set; }
    public PlayerAnimatorController playerAnimatorController { get; private set; }

    [Header("Camera")]
    public CinemachineCamera playerCamera;
    public Transform playerSwingTarget;
    public enum PlayerState
    {
        Walking,
        Swinging,
        Cutscene,
        Spawning
    }
    public PlayerState currentPlayerState {  get; private set; }

    void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }

        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerSwingController = GetComponent<PlayerSwingController>();
        playerCutsceneMovement = GetComponent<PlayerCutsceneMovement>();

        playerCamera.Priority = 10;

        StartMovement();
    }

    public void StartMovement()
    {
        playerCutsceneMovement.enabled = false;
        playerMovement.enabled = true;
        playerSwingController.enabled = false;
    }

    public void StartSwing(Transform swingPivot, SwingBarController currentBar, float forwardSpeed)
    {
        currentPlayerState = PlayerState.Swinging;

        playerMovement.enabled = false;
        playerSwingController.initialForwardSpeed = forwardSpeed;
        playerSwingController.enabled = true;
        playerSwingController.SetCurrentBar(swingPivot, currentBar);
        
        DropshadowController.Instance.DisableDropshadow();
        //ChangeCameraTracking(currentPlayerState);
    }

    public void EndSwing()
    {
        currentPlayerState = PlayerState.Walking;

        playerMovement.enabled = true;
        playerMovement.characterController.enabled = true;
        playerSwingController.enabled = false;
        playerMovement.Launch();
        DropshadowController.Instance.ActiveDropshadow();
        //ChangeCameraTracking(currentPlayerState);
    }

    private void ChangeCameraTracking(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Walking:
                playerCamera.Target.TrackingTarget = transform;
                break;
            case PlayerState.Swinging:
                playerCamera.Target.TrackingTarget = playerSwingTarget;
                break;
        }
    }

    public void ActivatePastLayer()
    {
        playerMovement.PastCollision();
    }

    public void ActivateFutureLayer()
    {
        playerMovement.FutureCollision();
    }

    public void StartCutsceneState()
    {
        playerMovement.enabled = false;
        playerCutsceneMovement.enabled = true;
    }
}
