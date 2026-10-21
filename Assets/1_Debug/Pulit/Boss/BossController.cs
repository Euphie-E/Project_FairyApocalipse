using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Estados do Animator")]
    public string idleState = "Base Layer.Idle";
    public string roarState = "Base Layer.Rugido";
    public string tail1State = "Base Layer.Cauda1";
    public string tail2State = "Base Layer.Cauda2";
    public string tail3State = "Base Layer.Cauda3";
    public string stompState = "Base Layer.PisadaForte";

    [Header("Duracao das Animacoes")]
    public float roarDuration = 2f;
    public float tail1Duration = 2f;
    public float tail2Duration = 2f;
    public float tail3Duration = 2f;
    public float stompDuration = 3f;

    [Header("Dano")]
    public BossDamageEvents damageEvents;

    [Header("Intervalo Aleatorio Entre Ataques")]
    [Min(0f)]
    public float minAttackDelay = 2f;

    [Min(0f)]
    public float maxAttackDelay = 5f;

    [Header("Transicao das Animacoes")]
    public float transitionTime = 0.15f;

    [Header("Eventos dos Terremotos")]
    public UnityEvent onWeakEarthquake;
    public UnityEvent onStrongEarthquake;

    [Header("Avisos dos Ataques")]
    public BossAttackWarningEvents warningEvents;

    private readonly HashSet<BossAttackZone> quakeZones = new();
    private readonly HashSet<BossAttackZone> activeAttackZones = new();

    private bool attacking;
    private float nextAttackTime;
    private BossAttackType currentAttack;

    [Header("VFX do Terremoto Forte")]
    [SerializeField] private VisualEffect strongEarthquakeVFX;

    [Header("VFX do Terremoto Fraco")]
    [SerializeField] private VisualEffect weakEarthquakeVFX;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    private Coroutine attackRoutine;
    private float attackStartTime;

    [Header("Tempo da Pisada Fraca")]
    [Range(0f, 1f)] public float ImpactWeak = 0.2f;

    [Header("Tempo da Pisada Forte")]
    [Range(0f, 1f)] public float ImpactStrong = 0.917f;


    [Header("Shake do Terremoto Fraco")]
    public float weakShakeMagnitude = 0.8f;
    public float weakShakeDuration = 0.3f;
    public float weakShakeFrequency = 2f;
    public float weakImpactTime = 1f;

    private bool impactTriggered;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Boss sem Animator", this);
            enabled = false;
            return;
        }

        animator.Play(idleState, 0, 0f);
    }

    private void Update()
    {
        if (attacking || Time.time < nextAttackTime)
            return;

        BossAttackType? nextAttack = GetActiveAttack();

        if (nextAttack.HasValue)
        {
            attackRoutine = StartCoroutine(
                ExecuteAttack(nextAttack.Value)
            );
        }
    }
    private BossAttackType? GetActiveAttack()
    {
        BossAttackType? tailAttack = null;

        foreach (BossAttackZone zone in activeAttackZones)
        {
            if (zone == null || !zone.isActiveAndEnabled)
                continue;

            
            if (zone.attackType == BossAttackType.PisadaForte)
                return BossAttackType.PisadaForte;

            if (!tailAttack.HasValue)
                tailAttack = zone.attackType;
        }

        
        if (tailAttack.HasValue)
            return tailAttack.Value;

        if (quakeZones.Count > 0)
            return BossAttackType.TerremotoFraco;

        return null;
    }

    public void EnterZone(BossAttackZone zone)
    {
        if (zone == null)
            return;

        Debug.Log("EnterZone: " + zone.name +
                  " tipo=" + zone.attackType);

        if (zone.attackType == BossAttackType.TerremotoFraco)
        {
            bool firstActivation =
                !attacking &&
                quakeZones.Count == 0 &&
                activeAttackZones.Count == 0;

            if (quakeZones.Add(zone) && firstActivation)
                ScheduleNextAttack();

            return;
        }

        if (!activeAttackZones.Add(zone))
            return;

        if (zone.attackType == BossAttackType.PisadaForte)
        {
            if (attacking &&
                currentAttack == BossAttackType.PisadaForte)
                return;

            if (attackRoutine != null)
                StopCoroutine(attackRoutine);

            if (damageEvents != null)
                damageEvents.DisableAll();

            attackRoutine = StartCoroutine(
                ExecuteAttack(BossAttackType.PisadaForte)
            );

            return;
        }

       
        if (!attacking)
            nextAttackTime = Time.time;
    }

    public void ExitZone(BossAttackZone zone)
    {
        if (zone == null)
            return;

        if (zone.attackType == BossAttackType.TerremotoFraco)
        {
            quakeZones.Remove(zone);
        }
        else
        {
            activeAttackZones.Remove(zone);
        }
    }

    private void ScheduleNextAttack()
    {
        float min = Mathf.Max(
            0f, Mathf.Min(minAttackDelay, maxAttackDelay)
        );

        float max = Mathf.Max(
            0f, Mathf.Max(minAttackDelay, maxAttackDelay)
        );

        float randomDelay = Random.Range(min, max);

        nextAttackTime = Time.time + randomDelay;
    }

    private IEnumerator WaitForNormalizedTime(string state, float target, float timeout)
    {
        int hash = Animator.StringToHash(state);
        float t = 0f;

        while (t < timeout)
        {
            AnimatorStateInfo info = animator.IsInTransition(0)
                ? animator.GetNextAnimatorStateInfo(0)
                : animator.GetCurrentAnimatorStateInfo(0);

            if (info.fullPathHash == hash && info.normalizedTime >= target)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator ExecuteAttack(BossAttackType type)
    {
        attacking = true;
        currentAttack = type;
        impactTriggered = false;
        attackStartTime = Time.time;

        string state = GetState(type);
        float duration = GetDuration(type);

        if (!animator.HasState(0, Animator.StringToHash(state)))
        {
            Debug.LogError("Estado nao encontrado: " + state, this);
            attacking = false;
            attackRoutine = null;
            nextAttackTime = Time.time + 1f;
            yield break;
        }

        if (type == BossAttackType.PisadaForte && strongEarthquakeVFX != null)
            strongEarthquakeVFX.Stop();

        if (type == BossAttackType.TerremotoFraco && weakEarthquakeVFX != null)
            weakEarthquakeVFX.Stop();

        damageEvents.DisableAll();
        warningEvents.HideAll();
        animator.CrossFadeInFixedTime(state, transitionTime, 0);

        if (type == BossAttackType.PisadaForte)
        {
            yield return WaitForNormalizedTime(state, ImpactStrong, 5f);
            DoStrongImpact();

            yield return WaitForNormalizedTime(state, 1f, 5f);
            animator.CrossFadeInFixedTime(idleState, transitionTime, 0);

            float elapsed = Time.time - attackStartTime;
            yield return new WaitForSeconds(Mathf.Max(0f, duration - elapsed));
        }
        else if (type == BossAttackType.TerremotoFraco)
        {
            yield return WaitForNormalizedTime(state, ImpactWeak, 5f);
            DoWeakImpact();

            yield return WaitForNormalizedTime(state, 1f, 5f);
            animator.CrossFadeInFixedTime(idleState, transitionTime, 0);

            float elapsed = Time.time - attackStartTime;
            yield return new WaitForSeconds(Mathf.Max(0f, duration - elapsed));
        }
        else
        {
            yield return new WaitForSeconds(Mathf.Max(0.1f, duration));
        }
        damageEvents.DisableAll();
        animator.CrossFadeInFixedTime(idleState, transitionTime, 0);

        warningEvents.HideAll();
        ScheduleNextAttack();

        attacking = false;
        attackRoutine = null;
    }

    private string GetState(BossAttackType type)
    {
        switch (type)
        {
            case BossAttackType.TerremotoFraco:
                return roarState;

            case BossAttackType.Cauda1:
                return tail1State;

            case BossAttackType.Cauda2:
                return tail2State;

            case BossAttackType.Cauda3:
                return tail3State;

            case BossAttackType.PisadaForte:
                return stompState;

            default:
                return idleState;
        }
    }

    private float GetDuration(BossAttackType type)
    {
        switch (type)
        {
            case BossAttackType.TerremotoFraco:
                return roarDuration;

            case BossAttackType.Cauda1:
                return tail1Duration;

            case BossAttackType.Cauda2:
                return tail2Duration;

            case BossAttackType.Cauda3:
                return tail3Duration;

            case BossAttackType.PisadaForte:
                return stompDuration;

            default:
                return 1f;
        }
    }

    
    private void DoStrongImpact()
    {
        Debug.Log("IMPACTO FORTE por tempo em " + (Time.time - attackStartTime).ToString("F2") + " s");

        onStrongEarthquake?.Invoke();
        warningEvents.PisadaAvisoOff();

        if (strongEarthquakeVFX != null)
        {
            strongEarthquakeVFX.Stop();
            strongEarthquakeVFX.Play();
        }

        if (cameraShake != null)
            cameraShake.Shake();
    }

    private void DoWeakImpact()
    {
        Debug.Log("IMPACTO FRACO por tempo em " + (Time.time - attackStartTime).ToString("F2") + " s");

        onWeakEarthquake?.Invoke();

        if (weakEarthquakeVFX != null)
        {
            weakEarthquakeVFX.Stop();
            weakEarthquakeVFX.Play();
        }

        if (cameraShake != null)
            cameraShake.Shake(weakShakeMagnitude, weakShakeDuration, weakShakeFrequency);
    }

    public void Event_WeakEarthquake()
    {
        Debug.Log("Animation Event WEAK ignorado");
    }

    public void Event_StrongEarthquake()
    {
        Debug.Log("Animation Event STRONG ignorado");
    }
}
