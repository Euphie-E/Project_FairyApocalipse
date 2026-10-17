
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BossDamageTrigger : MonoBehaviour
{
    [Header("Estado do Dano")]
    [SerializeField] private bool damageActive = false;

    private BoxCollider damageCollider;

    private void Awake()
    {
        damageCollider = GetComponent<BoxCollider>();
        damageCollider.isTrigger = true;

        damageActive = false;
    }

    public void SetDamageActive(bool active)
    {
        damageActive = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (!damageActive)
            return;

        PlayerDeathController player =
            other.GetComponentInParent<PlayerDeathController>();

        if (player == null)
            return;

        damageActive = false;

        player.KillFromBoss();
    }

    private void OnDisable()
    {
        damageActive = false;
    }
}
