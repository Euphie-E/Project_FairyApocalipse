using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.VFX;

public class PlayerDeathController : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int BaseLayerDeathTreeHash = Animator.StringToHash("Base Layer.DeathTree");
    private static readonly int DeathValueHash = Animator.StringToHash("DeathValue");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static int respawnAnimatorStates;
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
    private string respawnAnimatorStatesSTR =
    "Base Layer.Ground Movement";

    [Header("Boss Knockback")]
    [SerializeField] private float bossPushDistance = 1.5f;
    [SerializeField] private float bossPushDuration = 0.2f;

    [Header("Boss Tongue Pull")]
    [SerializeField] private float bossPullDistance = 1.5f;
    [SerializeField] private float bossPullDuration = 0.2f;

    [Header("Tear Push")]
    [SerializeField] private float pushDistance = 1.5f;
    [SerializeField] private float pushDuration = 0.2f;

    private bool isDrowning = false;

    private Coroutine bossPushCoroutine;
    private Coroutine bossPullCoroutine;
    private Coroutine pushCoroutine;


    [Header("Revive")]
    [SerializeField] private float toReviveTime = 3f;
    private float toReviveTimer = 0f;

    [Header("Protecao Respawn")]
    [SerializeField] private float respawnProtection = 2f;

    private float protectedUntil = -1f;

    private bool IsRespawnProtected()
    {
        return Time.time < protectedUntil;
    }

    private void ProtectRespawn()
    {
        time = 0f;
        isFalling = false;
        fallHeight = 0f;
        yfall = transform.position.y;

        protectedUntil = Time.time + respawnProtection;
    }
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

       /*  checkpoints = new GameObject[GameObject.FindGameObjectsWithTag("Checkpoint").Length + 1];

        checkpoints[0] = transform.parent.gameObject; // colocar o spawn como inicio, meio gambiarra aproveitando que o player pai não se mexe;
        DataManager.Instance?.SetTotalCheckpoints(checkpoints.Length); */
        LevelManager.Get()?.SetPlayer(this.gameObject);
        respawnAnimatorStates = Animator.StringToHash(respawnAnimatorStatesSTR);
    }

    float max = 3;
    public float time;
    bool z = false;
    void Update()
    {
        if (isDead || IsRespawnProtected())
        {
            time = 0f;
            return;
        }
        if (isDead)
            return;

        // Durante o Push não contabiliza queda
        if (pushCoroutine != null)
        {
            time = 0f;
            return;
        }

        if (playerMovement.enabled &&
            !playerMovement.animatorGrounded &&
            playerMovement.verticalVelocity < 0)
        {
            time += Time.deltaTime;
        }
        else
        {
            time -= Time.deltaTime * 0.6f;

            if (time < max * 0.5f)
                time = 0f;
        }

        if (time >= max)
        {
            Debug.LogWarning("PLAYER MORREU POR TEMPO DE QUEDA");

            isDead = true;
            time = 0f;

            playerInput.OnDeath();
            StartDeathSequence();
        }
    }

    /* private void CheckFall()
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
    } */

    /* private void CheckDeath()
    {
        if (isDead && (playerMovement.isGrounded || fallHeight > maxHeight*2.5f || isDrowning))
        {
            playerInput.OnDeath();
            toReviveTimer += Time.deltaTime;
            if (toReviveTimer >= toReviveTime)
            {
                toReviveTimer = 0;
                Revive();
            }
        }
    } */

    private void StartDeathSequence()
    {
        if (deathSequenceCoroutine != null)
            StopCoroutine(deathSequenceCoroutine);

        playerMovement.ResetMotionForDeath();
        //playerMovement.enabled = false;

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
        time = 0;

        ResetDeathDissolve();

        /* playerCharacterController.enabled = false;

        Vector3 respawnPosition =
            checkpoints[caughtCheckpointIndex].transform.position
            + Vector3.up * 1.5f;

        Vector3 difference = respawnPosition - transform.position;

        transform.position = respawnPosition;

        playerCharacterController.enabled = true; */

        /*playerMovement.enabled = true;

         cinemachineCamera.OnTargetObjectWarped(
            cinemachineCamera.Target.TrackingTarget,
            difference
        ); */

        ProtectRespawn();

        playerMovement.TeleportLastCheckpoint();

        ProtectRespawn();

        Debug.Log(
            "[RESPAWN] Player renasceu em: " + transform.position
        );

        TimeTravel timeTravel = GetComponent<TimeTravel>();

        if (timeTravel != null)
        {
            timeTravel.ResetAfterRespawn();
        }

        playerInput.OnRevive();
        deathAnimator.ResetTrigger(DeathHash);
        deathAnimator.SetFloat(SpeedHash, 0f);

        if (deathAnimator.HasState(
            0, respawnAnimatorStates))
        {
            deathAnimator.Play(respawnAnimatorStates, 0, 0f);
            deathAnimator.SetFloat(DeathValueHash, 0f);
        }
        else
        {
            Debug.LogWarning(
                "Estado de respawn nao encontrado: " +
                respawnAnimatorStates
            );
        }

        /* Checkpoint checkpoint =
            checkpoints[caughtCheckpointIndex].GetComponent<Checkpoint>(); */

        

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
        ProtectRespawn();
    }

    public void KillFromBoss()
    {
        if (isDead || IsRespawnProtected())
            return;

        Debug.LogError("[MORTE] KILL FROM BOSS!");

        deathAnimator.ResetTrigger(DeathHash);
        deathAnimator.SetFloat(DeathValueHash, 1f);
        deathAnimator.Play(BaseLayerDeathTreeHash, 0, 0f);

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

    public void Pull(Vector3 hitPosition)
    {
        if (playerCharacterController == null || !playerCharacterController.enabled)
        {
            return;
        }

        //hitPosition.y = transform.position.y;
        Vector3 direction = hitPosition - transform.position;
        //direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = -transform.forward;
        }

        direction.Normalize();

        if (bossPullCoroutine != null)
        {
            StopCoroutine(bossPullCoroutine);
        }

        bossPullCoroutine = StartCoroutine(BossPullRoutine(direction));
    }

    public void Push(Vector3 hitPosition)
    {
        if (isDead ||
            playerCharacterController == null ||
            !playerCharacterController.enabled)
            return;

        Vector3 direction = transform.position - hitPosition;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            direction = -transform.forward;

        direction.Normalize();

        if (pushCoroutine != null)
            StopCoroutine(pushCoroutine);

        time = 0f;

        pushCoroutine = StartCoroutine(pushRoutine(direction));
    }

    private IEnumerator pushRoutine(Vector3 direction)
    {
        float timer = 0f;
        float previousProgress = 0f;
        float duration = Mathf.Max(0.01f, pushDuration);

        while (timer < duration)
        {
            if (isDead || !playerCharacterController.enabled)
                break;

            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            float progress = 1f - Mathf.Pow(1f - t, 2f);

            float distanceStep =
                (progress - previousProgress) * pushDistance;

            playerCharacterController.Move(direction * distanceStep);

            previousProgress = progress;

            // Impede que o empurrão acumule tempo de queda
            time = 0f;

            yield return null;
        }

        pushCoroutine = null;
    }

    private IEnumerator BossPullRoutine(Vector3 direction)
    {
        float timer = 0f;
        float previousProgress = 0f;
        float duration = Mathf.Max(0.01f, bossPullDuration);

        while (timer < duration)
        {
            if (!playerCharacterController.enabled) break;

            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            float progress = 1f - Mathf.Pow(1f - t, 2f);

            float distanceStep = (progress - previousProgress) * bossPullDistance;

            playerCharacterController.Move(direction * distanceStep);

            previousProgress = progress;

            yield return null;
        }

        bossPullCoroutine = null;
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

    public void Drown()
    {

        Debug.LogError(
    "[DROWN CHAMADO]\n" +
    "Player: " + transform.position + "\n" +
    "Origem da chamada:\n" +
    System.Environment.StackTrace,
    gameObject
);
        if (isDead || IsRespawnProtected())
            return;

        Debug.LogError(
            "[MORTE] DROWN! Posicao: " + transform.position
        );

        isDrowning = true;
        isDead = true;
        time = 0f;

        StartDeathSequence();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathBox") && !isDead)
        {
            Debug.LogError(
                "[DEATHBOX DETECTADA]\n" +
                "Objeto: " + other.gameObject.name + "\n" +
                "Objeto Pai: " + other.transform.root.name + "\n" +
                "Cena: " + other.gameObject.scene.name + "\n" +
                "Layer: " + LayerMask.LayerToName(other.gameObject.layer) + "\n" +
                "Posicao DeathBox: " + other.bounds.center + "\n" +
                "Posicao Player: " + transform.position,
                other.gameObject
            );

            Drown();
        }
    }
}
