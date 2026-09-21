using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDeathController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private CharacterController playerCharacterController;
    [SerializeField] private GameObject[] checkpoints;
    private int caughtCheckpointIndex = 0;
    [SerializeField] private float maxHeight = 25f;
    [SerializeField] private bool isFalling = false;
    private float yfall;
    private float fallHeight = 0;
    private bool isDead = false;   

    [SerializeField] private const float toReviveTime = 2f;
    private float toReviveTimer = 0f ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerCharacterController = gameObject.GetComponent<CharacterController>();

        checkpoints = new GameObject[GameObject.FindGameObjectsWithTag("Checkpoint").Length + 1];

        checkpoints[0] = transform.parent.gameObject; // colocar o spawn como inicio, meio gambiarra aproveitando que o player pai não se mexe;
    }

    void Update()
    {
        CheckFall();
        CheckFallDistance();
        CheckDeath();
    }

    private void CheckFall()
    {
        if (!isDead && !isFalling && playerMovement.verticalVelocity < 0) 
        {
            isFalling = true;
            yfall = transform.position.y;
        }

        else if (!isDead && isFalling && playerMovement.verticalVelocity >= 0) 
        {
            CheckFallDeath();
            isFalling = false;
            fallHeight = 0;
        }
    }

    private void CheckFallDistance()
    {
        if (!isDead && isFalling)
        {
            fallHeight = yfall - transform.position.y;
            
            CheckFallDeath();
        }
    }

    private void CheckFallDeath()
    {
        if (!isDead && isFalling && fallHeight > maxHeight)
        {
            isDead = true;
        }
    }

    private void CheckDeath()
    {
        if (isDead && playerMovement.isGrounded)
        {
            toReviveTimer += Time.deltaTime;
            if (toReviveTimer >= toReviveTime)
            {
                playerInput.OnDeath();
                toReviveTimer = 0;
                Revive();
            }
        }
    }

    private void Revive()
    {
        isFalling = false;
        fallHeight = 0;
        toReviveTimer = 0;
        playerCharacterController.enabled = false;
        transform.position = checkpoints[caughtCheckpointIndex].transform.position + Vector3.up * 1.5f;
        playerCharacterController.enabled = true;
        playerInput.OnRevive();
        isDead = false;
    }

    public void NewCheckpoint(int index, GameObject checkpoint)
    {
        caughtCheckpointIndex = index;
        checkpoints[index] = checkpoint;
    }
}
