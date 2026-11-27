
using UnityEngine;

public class GnomeCollectible : MonoBehaviour
{
    [Header("Identificacao")]
    [SerializeField, Range(1, 14)]
    private int gnomeId = 1;

    private void Start()
    {
        if (GnomeProgress.IsCollected(gnomeId))
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Registra a coleta apenas uma vez.
        if (!GnomeProgress.Collect(gnomeId))
            return;

        // Exibe a contagem da fase.
        if (GnomeCollectionNotice.Instance != null)
        {
            GnomeCollectionNotice.Instance
                .ShowCollected(gnomeId);
        }

        gameObject.SetActive(false);
    }
}
