
using System.Collections.Generic;
using UnityEngine;

public enum BossAttackType
{
    TerremotoFraco,
    Cauda1,
    Cauda2,
    Cauda3,
    PisadaForte
}

[RequireComponent(typeof(Collider))]
public class BossAttackZone : MonoBehaviour
{
    public BossController boss;
    public BossAttackType attackType;

    private readonly HashSet<Collider> collidersDoPlayer = new();

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTROU NO TRIGGER: " + other.name);
        if (!other.transform.root.CompareTag("Player"))
            return;

        if (collidersDoPlayer.Add(other) &&
            collidersDoPlayer.Count == 1)
        {
            if (boss != null)
                boss.EnterZone(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!collidersDoPlayer.Remove(other))
            return;

        if (collidersDoPlayer.Count == 0 && boss != null)
            boss.ExitZone(this);
    }

    private void OnDisable()
    {
        if (collidersDoPlayer.Count > 0 && boss != null)
            boss.ExitZone(this);

        collidersDoPlayer.Clear();
    }
}
