using System.Collections;
using UnityEngine;

public class TearShot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DebrisSpawner spawner;

    [Header("Settings")]
    [SerializeField] private float finishDelay = 2f;

    private bool finishing = false;

    private void OnEnable()
    {
        finishing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathController>()
                .Push(transform.position);
        }

        if (!finishing)
        {
            StartCoroutine(FinishDropDelayed());
        }
    }

    private IEnumerator FinishDropDelayed()
    {
        finishing = true;

        yield return new WaitForSeconds(finishDelay);

        spawner.FinishDrop();
    }
}