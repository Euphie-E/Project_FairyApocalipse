
using System.Collections.Generic;
using UnityEngine;
public interface IMushroomBounceReceiver
{
    void BounceFromMushroom(float height);
}

[RequireComponent(typeof(Collider))]
public class MushroomBounce : MonoBehaviour
{
    [Header("Impulso")]
    [Min(0.1f)]
    public float bounceHeight = 5f;

    [Header("Deteccao")]
    [Min(0f)]
    public float minimumFallSpeed = 0.1f;

    [Min(0f)]
    public float landingTolerance = 0.25f;

    [Min(0f)]
    public float cooldown = 0.3f;

    [Header("Som (Opcional)")]
    public AudioSource audioSource;
    public AudioClip bounceSound;

    [Header("Efeito Visual")]
    public MushroomSpringVisual springVisual;


    private Collider triggerCollider;

    private readonly HashSet<CharacterController>
        playersInside = new();

    private float nextBounceTime;
    private bool warnedMissingReceiver;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryBounce(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryBounce(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController player =
            other.GetComponentInParent<CharacterController>();

        if (player != null)
            playersInside.Remove(player);
    }

    private void TryBounce(Collider other)
    {
        CharacterController player =
            other.GetComponentInParent<CharacterController>();

        if (player == null)
            return;

        if (playersInside.Contains(player))
            return;

        if (Time.time < nextBounceTime)
            return;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (movement == null)
            return;

        // Nao ativa enquanto estiver subindo
        if (movement.verticalVelocity > 0f)
            return;

        // Comunica o impulso ao movimento do player.
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
                    "IMushroomBounceReceiver.", this
                );

                warnedMissingReceiver = true;
            }

            return;
        }

        playersInside.Add(player);
        nextBounceTime = Time.time + cooldown;

        springVisual.Play();

        // Colocar som aqui 
       
    }

    private void OnDisable()
    {
        playersInside.Clear();
    }
}
