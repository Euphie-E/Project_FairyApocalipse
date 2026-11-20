
using System.Collections.Generic;
using UnityEngine;

public class MushroomTransformBounce : MonoBehaviour
{
    [Header("Impulso")]
    [Min(0.1f)]
    public float bounceHeight = 5f;

    [Header("Deteccao")]
    [Min(0f)]
    public float cooldown = 0.3f;

    [Header("Som (Opcional)")]
    public AudioSource audioSource;
    public AudioClip bounceSound;

    [Header("Efeito Visual")]
    public MushroomTransformSpringVisual springVisual;

    private BoxCollider[] trampolineColliders;

    private readonly Dictionary<CharacterController, int>
        overlapCounts = new();

    private readonly HashSet<CharacterController>
        playersBounced = new();

    private float nextBounceTime;
    private bool warnedMissingReceiver;

    private void Awake()
    {
        // Pega todos os Box Colliders deste objeto
        trampolineColliders = GetComponents<BoxCollider>();

        if (trampolineColliders.Length == 0)
        {
            Debug.LogWarning(
                "Nenhum BoxCollider encontrado no trampolim!",
                this
            );
        }

        // Todos funcionam como trigger
        foreach (BoxCollider col in trampolineColliders)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController player =
            other.GetComponentInParent<CharacterController>();

        if (player == null)
            return;

        // Conta quantos colliders estao tocando o player
        if (!overlapCounts.ContainsKey(player))
            overlapCounts[player] = 0;

        overlapCounts[player]++;

        TryBounce(player);
    }

    private void OnTriggerStay(Collider other)
    {
        CharacterController player =
            other.GetComponentInParent<CharacterController>();

        if (player == null)
            return;

        TryBounce(player);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController player =
            other.GetComponentInParent<CharacterController>();

        if (player == null)
            return;

        if (!overlapCounts.ContainsKey(player))
            return;

        overlapCounts[player]--;

        // So libera quando sair de todos os colliders
        if (overlapCounts[player] <= 0)
        {
            overlapCounts.Remove(player);
            playersBounced.Remove(player);
        }
    }

    private void TryBounce(CharacterController player)
    {
        if (playersBounced.Contains(player))
            return;

        if (Time.time < nextBounceTime)
            return;

        PlayerMovement movement =
            player.GetComponent<PlayerMovement>();

        if (movement == null)
            return;

        // Nao ativa enquanto estiver subindo
        if (movement.verticalVelocity > 0f)
            return;

        bool launched = false;

        MonoBehaviour[] components =
            player.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IMushroomBounceReceiver receiver)
            {
                receiver.BounceFromMushroom(bounceHeight);

                launched = true;
                break;
            }
        }

        if (!launched)
        {
            if (!warnedMissingReceiver)
            {
                Debug.LogWarning(
                    "O player precisa implementar " +
                    "IMushroomBounceReceiver.",
                    this
                );

                warnedMissingReceiver = true;
            }

            return;
        }

        playersBounced.Add(player);
        nextBounceTime = Time.time + cooldown;

        // Animacao do cogumelo
        if (springVisual != null)
            springVisual.Play();

        // Som
        if (audioSource != null && bounceSound != null)
            audioSource.PlayOneShot(bounceSound);
    }

    private void OnDisable()
    {
        overlapCounts.Clear();
        playersBounced.Clear();
    }
}
