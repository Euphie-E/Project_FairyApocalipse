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

        playerMovement = GetComponent<PlayerMovement>();
        playerSwingController = GetComponent<PlayerSwingController>();

        playerCamera.Priority = 10;
    }

    public void StartMovement()
    {
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
        //ChangeCameraTracking(currentPlayerState);
    }

    public void EndSwing()
    {
        currentPlayerState = PlayerState.Walking;

        playerMovement.enabled = true;
        playerMovement.characterController.enabled = true;
        playerSwingController.enabled = false;
        playerMovement.Launch();
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
}
