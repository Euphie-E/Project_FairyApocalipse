
using UnityEngine;

public class GnomeCollectible : MonoBehaviour
{
    [Header("Identificação")]
    [SerializeField, Range(1, 14)]
    private int gnomeId = 1;

    private void Start()
    {
        if (GnomeProgress.IsCollected(gnomeId))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GnomeProgress.Collect(gnomeId))
        {
            gameObject.SetActive(false);
        }
    }
}
