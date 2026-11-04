using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.VFX;

public class PlayerDeathController : MonoBehaviour
{
    public PlayerMovement playerMovement;
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


    [Header("Death VFX")]
    [SerializeField] private VisualEffect deathVFX;

    [Header("Death Dissolve")]
    [SerializeField] private Renderer[] dissolveRenderers;
    [SerializeField] private float dissolveDuration = 1f;
    private Coroutine dissolveCoroutine;
    private static readonly int DissolveCutID =
        Shader.PropertyToID("_DissolveCut");
    private MaterialPropertyBlock propertyBlock;

    [Header("Death Sequence")]
    [SerializeField] private float deathVFXDuration = 1.5f;

    private Coroutine deathSequenceCoroutine;


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

        propertyBlock = new MaterialPropertyBlock();
        if (dissolveRenderers == null || dissolveRenderers.Length == 0)
        {
            dissolveRenderers = GetComponentsInChildren<Renderer>(true);
        }
        SetDissolveValue(2f);

        checkpoints = new GameObject[GameObject.FindGameObjectsWithTag("Checkpoint").Length + 1];

        checkpoints[0] = transform.parent.gameObject; // colocar o spawn como inicio, meio gambiarra aproveitando que o player pai não se mexe;
        DataManager.Instance?.SetTotalCheckpoints(checkpoints.Length);
        if(LevelManager.Instance != null)LevelManager.Instance.Player = this.gameObject;
    }

    
    void Update()
    {
        CheckFall();
        CheckFallDistance();

        if (isDead)
        {
            playerInput.OnDeath();
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
            isFalling = false;

            StartDeathSequence();
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

    private void StartDeathSequence()
    {
        if (deathSequenceCoroutine != null)
            StopCoroutine(deathSequenceCoroutine);

        playerMovement.ResetMotionForDeath();
        playerMovement.enabled = false;

        PlayDeathDissolve();

        if (deathVFX != null)
        {
            deathVFX.Stop();
            deathVFX.Reinit();
            deathVFX.Play();
        }

        deathSequenceCoroutine = StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        float totalDuration = Mathf.Max(
            dissolveDuration,
            deathVFXDuration
        );

        yield return new WaitForSeconds(totalDuration);

        deathSequenceCoroutine = null;

        Revive();
    }

    private void Revive()
    {
        isFalling = false;
        fallHeight = 0f;
        toReviveTimer = 0f;

        ResetDeathDissolve();

        playerCharacterController.enabled = false;

        Vector3 respawnPosition =
            checkpoints[caughtCheckpointIndex].transform.position
            + Vector3.up * 1.5f;

        Vector3 difference = respawnPosition - transform.position;

        transform.position = respawnPosition;

        playerCharacterController.enabled = true;

        playerMovement.ResetMotionForDeath();
        playerMovement.enabled = true;

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

        StartDeathSequence();
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

    private void PlayDeathDissolve()
    {
        if (dissolveCoroutine != null)
            StopCoroutine(dissolveCoroutine);

        dissolveCoroutine = StartCoroutine(DeathDissolveRoutine());
    }

    private IEnumerator DeathDissolveRoutine()
    {
        float timer = 0f;

        SetDissolveValue(1f);

        while (timer < dissolveDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / dissolveDuration);
            float dissolveValue = Mathf.Lerp(1f, -2f, progress);

            SetDissolveValue(dissolveValue);
            yield return null;
        }

        SetDissolveValue(0f);
        dissolveCoroutine = null;
    }

    private void SetDissolveValue(float value)
    {
        foreach (Renderer rend in dissolveRenderers)
        {
            if (rend == null)
                continue;

            Material[] materials = rend.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null)
                    continue;

                rend.GetPropertyBlock(propertyBlock, i);
                propertyBlock.SetFloat(DissolveCutID, value);
                rend.SetPropertyBlock(propertyBlock, i);
            }
        }
    }

    private void ResetDeathDissolve()
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
            dissolveCoroutine = null;
        }

        SetDissolveValue(2f);
    }
}
