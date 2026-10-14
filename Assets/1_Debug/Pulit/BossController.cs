
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

    private readonly HashSet<BossAttackZone> quakeZones = new();
    private readonly Queue<BossAttackType> pendingAttacks = new();

    private bool attacking;
    private float nextAttackTime;
    private BossAttackType currentAttack;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Boss sem Animator!", this);
            enabled = false;
            return;
        }

        animator.Play(idleState, 0, 0f);
    }

    private void Update()
    {
        if (attacking || Time.time < nextAttackTime)
            return;

        if (pendingAttacks.Count > 0)
        {
            StartCoroutine(
                ExecuteAttack(pendingAttacks.Dequeue())
            );
        }
        else if (quakeZones.Count > 0)
        {
            StartCoroutine(
                ExecuteAttack(BossAttackType.TerremotoFraco)
            );
        }
    }

    public void EnterZone(BossAttackZone zone)
    {
        // NOVO: aplica espera aleatoria na primeira
        // entrada em uma regiao de ataque.
        bool firstActivation =
            !attacking &&
            quakeZones.Count == 0 &&
            pendingAttacks.Count == 0;

        if (firstActivation)
            ScheduleNextAttack();

        if (zone.attackType == BossAttackType.TerremotoFraco)
        {
            quakeZones.Add(zone);
        }
        else
        {
            pendingAttacks.Enqueue(zone.attackType);
        }
    }

    public void ExitZone(BossAttackZone zone)
    {
        if (zone.attackType == BossAttackType.TerremotoFraco)
            quakeZones.Remove(zone);
    }

    // NOVO: sorteia o tempo de espera.
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

    private IEnumerator ExecuteAttack(BossAttackType type)
    {
        attacking = true;
        currentAttack = type;

        string state = GetState(type);
        float duration = GetDuration(type);

        if (!animator.HasState(0, Animator.StringToHash(state)))
        {
            Debug.LogError("Estado nao encontrado: " + state, this);
            attacking = false;
            nextAttackTime = Time.time + 1f;
            yield break;
        }

        animator.CrossFadeInFixedTime(
            state, transitionTime, 0
        );

        yield return new WaitForSeconds(
            Mathf.Max(0.1f, duration)
        );

        animator.CrossFadeInFixedTime(
            idleState, transitionTime, 0
        );

        // NOVO: sorteia outro intervalo depois do ataque.
        ScheduleNextAttack();

        attacking = false;
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

    // Chamados por Animation Events nos frames de impacto.

    public void Event_WeakEarthquake()
    {
        if (attacking &&
            currentAttack == BossAttackType.TerremotoFraco &&
            quakeZones.Count > 0)
        {
            onWeakEarthquake?.Invoke();
        }
    }

    public void Event_StrongEarthquake()
    {
        if (attacking &&
            currentAttack == BossAttackType.PisadaForte)
        {
            onStrongEarthquake?.Invoke();
        }
    }
}
