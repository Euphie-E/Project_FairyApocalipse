
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BossTimeTravelBlockZone : MonoBehaviour
{
    [Header("Sistema de Viagem Temporal")]
    public TimeTravel timeTravel;

    private readonly HashSet<Collider> playerColliders = new();

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponentInParent<PlayerMovement>() == null)
        //    return;
        //
        //if (playerColliders.Add(other) &&
        //    playerColliders.Count == 1)
        //{
        //    if (timeTravel != null)
        //        timeTravel.SetTravelBlocked(true);
        //
        //    Debug.Log("Viagem temporal bloqueada");
        //}
        if (other.CompareTag("Player"))
        {
            timeTravel = other.GetComponent<TimeTravel>();
            timeTravel.SetTravelBlocked(true);
            Debug.Log("Viagem temporal bloqueada");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!playerColliders.Remove(other))
        //    return;
//
        //if (playerColliders.Count == 0)
        //{
        //    if (timeTravel != null)
        //        timeTravel.SetTravelBlocked(false);
//
        //    Debug.Log("Viagem temporal liberada");
        //}
        if (other.CompareTag("Player"))
        {
            if (timeTravel != null)
                timeTravel.SetTravelBlocked(false);
            Debug.Log("Viagem temporal bloqueada");
        }
    }

    private void OnDisable()
    {
        playerColliders.Clear();

        if (timeTravel != null)
            timeTravel.SetTravelBlocked(false);
    }
}
