using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using Unity.Cinemachine;

public class PlayerDeathController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private CharacterController playerCharacterController;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GameObject[] checkpoints;
    private int caughtCheckpointIndex = 0;
    [SerializeField] private float maxHeight = 25f;
    [SerializeField] private bool isFalling = false;
    private float yfall;
    private float fallHeight = 0;
    private bool isDead = false;

    [Header("Boss Death")]
    [SerializeField] private Animator deathAnimator;
    [Header("Retorno da Animacao")]
    [SerializeField]
    private string respawnAnimatorStates =
    "Base Layer.Ground Movement";

    [Header("Boss Knockback")]
    [SerializeField] private float bossPushDistance = 1.5f;
    [SerializeField] private float bossPushDuration = 0.2f;

    private Coroutine bossPushCoroutine;


    [SerializeField] private float toReviveTime = 3f;
    private float toReviveTimer = 0f;

    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerCharacterController = gameObject.GetComponent<CharacterController>();

        checkpoints = new GameObject[GameObject.FindGameObjectsWithTag("Checkpoint").Length + 1];

        checkpoints[0] = transform.parent.gameObject; // colocar o spawn como inicio, meio gambiarra aproveitando que o player pai não se mexe;
        DataManager.Instance?.SetTotalCheckpoints(checkpoints.Length);
    }

    
    void Update()
    {
        CheckFall();
        CheckFallDistance();

        if (isDead)
        {
            playerInput.OnDeath();

            if (playerMovement.isGrounded || fallHeight > maxHeight * 2.5f)
            {
                toReviveTimer += Time.deltaTime;

                if (toReviveTimer >= toReviveTime)
                {
                    Revive();
                }
            }
        }
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
            CheckFallDistance();
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
        if (!isDead && isFalling && fallHeight > maxHeight)
        {
            isDead = true;
        }
    }

    private void CheckDeath()
    {
        if (isDead && (playerMovement.isGrounded || fallHeight > maxHeight*2.5f))
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
        isFalling = false;
        fallHeight = 0f;
        toReviveTimer = 0f;

        playerCharacterController.enabled = false;

        Vector3 respawnPosition =
            checkpoints[caughtCheckpointIndex].transform.position
            + Vector3.up * 1.5f;

        Vector3 difference = respawnPosition - transform.position;

        transform.position = respawnPosition;

        playerCharacterController.enabled = true;

        cinemachineCamera.OnTargetObjectWarped(
            cinemachineCamera.Target.TrackingTarget,
            difference
        );

        playerInput.OnRevive();

        deathAnimator.ResetTrigger("Death");
        deathAnimator.SetFloat("Speed", 0f);

        if (deathAnimator.HasState(
            0, Animator.StringToHash(respawnAnimatorStates)))
        {
            deathAnimator.Play(respawnAnimatorStates, 0, 0f);
            deathAnimator.SetFloat("DeathValue", 0f);
        }
        else
        {
            Debug.LogWarning(
                "Estado de respawn nao encontrado: " +
                respawnAnimatorStates
            );
        }

        Checkpoint checkpoint =
            checkpoints[caughtCheckpointIndex].GetComponent<Checkpoint>();

        if (checkpoint != null)
        {
            checkpoint.PlayRespawnEffect();
        }

        isDead = false;
    }

    public void NewCheckpoint(int index, GameObject checkpoint)
    {
        caughtCheckpointIndex = index;
        checkpoints[index] = checkpoint;
        if (DataManager.Instance != null)
            DataManager.Instance.AddData(DataManager.Data.TimeStamp,Time.time,index);
    }

    public void ResetFallAfterTeleport()
    {
        isFalling = false;
        fallHeight = 0f;
        yfall = transform.position.y;
    }

    public void KillFromBoss()
    {
        if (isDead)
            return;

        deathAnimator.ResetTrigger("Death");
        deathAnimator.SetFloat("DeathValue", 1f);
        deathAnimator.Play("Base Layer.DeathTree", 0, 0f);

        isFalling = false;
        fallHeight = 0f;
        yfall = transform.position.y;

        toReviveTimer = 0f;

        isDead = true;
    }

    public void PushAwayFromBoss(Vector3 hitPosition)
    {
        if (playerCharacterController == null ||
            !playerCharacterController.enabled)
            return;

        Vector3 direction = transform.position - hitPosition;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = -transform.forward;

        direction.Normalize();

        if (bossPushCoroutine != null)
            StopCoroutine(bossPushCoroutine);

        bossPushCoroutine = StartCoroutine(
            BossPushRoutine(direction)
        );
    }

    private IEnumerator BossPushRoutine(Vector3 direction)
    {
        float timer = 0f;
        float previousProgress = 0f;
        float duration = Mathf.Max(0.01f, bossPushDuration);

        while (timer < duration && isDead)
        {
            if (!playerCharacterController.enabled)
                break;

            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

       
            float progress = 1f - Mathf.Pow(1f - t, 2f);

            float distanceStep =
                (progress - previousProgress) * bossPushDistance;

            playerCharacterController.Move(
                direction * distanceStep
            );

            previousProgress = progress;

            yield return null;
        }

        bossPushCoroutine = null;
    }
}
