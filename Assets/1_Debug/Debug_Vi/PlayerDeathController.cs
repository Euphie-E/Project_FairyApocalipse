using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDeathController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    [SerializeField] private GameObject[] checkpoints;
    private int caughtCheckpointIndex = 0;
    [SerializeField] private float maxHeight = 25f;
    [SerializeField] private bool isFalling = false;
    private float yfall;
    private float fallHeight = 0;
    private bool isDead = false;   

    [SerializeField] private const float toReviveTime = 0.1f;
    private float toReviveTimer = 0f ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerInput = gameObject.GetComponent<PlayerInput>();

        checkpoints = new GameObject[GameObject.FindGameObjectsWithTag("Checkpoint").Length + 1];

        checkpoints[0] = transform.parent.gameObject; // colocar o spawn como inicio, meio gambiarra aproveitando que o player pai não se mexe;
    }

    //gambi
    void OnEnable()
    {
        isFalling = false;
        yfall = 0;
        fallHeight = 0;
        isDead = false;
    }

    void Update()
    {
        CheckFall();
        CheckFallDistance();
        CheckDeath();
    }

    private void CheckFall()
    {
        if (!isFalling && playerMovement.verticalVelocity < 0) 
        {
            isFalling = true;
            yfall = transform.position.y;
        }

        else if (isFalling && playerMovement.verticalVelocity >= 0) 
        {
            CheckFallDeath();
            isFalling = false;
            fallHeight = 0;
        }
    }

    private void CheckFallDistance()
    {
        if (isFalling)
        {
            fallHeight = yfall - transform.position.y;
            
            CheckFallDeath();
        }
    }

    private void CheckFallDeath()
    {
        if (isFalling && fallHeight > maxHeight)
        {
            isDead = true;
        }
    }

    private void CheckDeath()
    {
        if (isDead && playerMovement.isGrounded)
        {
            playerInput.OnDeath();

            toReviveTimer += Time.deltaTime;
            if (toReviveTimer >= toReviveTime)
            {
                toReviveTimer = 0;
                Revive();
            }
        }
    }

    private void Revive()
    {
        //gambi
        transform.GetComponent<TimeTravel>().HandleReset();
        //gambi
        transform.position = checkpoints[caughtCheckpointIndex].transform.position;
        playerInput.OnRevive();
    }

    public void NextCheckpoint(int index, GameObject checkpoint)
    {
        caughtCheckpointIndex = index;
        checkpoints[index] = checkpoint;
    }
}
