
using UnityEngine;

public class GnomeCollectible : MonoBehaviour
{
    [Header("Identificação")]
    [SerializeField, Range(1, 14)]
    private int gnomeId = 1;

    [Header("Notificação")]
    [SerializeField] private NotifData data;

    private void Start()
    {
        // Se ja foi coletado, nao aparece na cena.
        if (GnomeProgress.IsCollected(gnomeId))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Se ja foi coletado, ignora.
        if (!GnomeProgress.Collect(gnomeId))
            return;

        // Exibe a notificacao.
        if (NotificationManager.instance != null &&
            data != null)
        {
            NotificationManager.instance.ShowNotif(data);
        }

        Debug.Log($"[GNOMO] Gnomo {gnomeId} coletado!");

        // Desativa o gnomo da cena.
        gameObject.SetActive(false);
    }
}
