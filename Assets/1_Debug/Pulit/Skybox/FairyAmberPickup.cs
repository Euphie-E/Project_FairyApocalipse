
using UnityEngine;

public class FairyAmberPickup : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private GameObject amber;
    [SerializeField] private GameObject fairy;

    [Header("Time Travel")]
    [SerializeField] private TimeTravel timeTravel;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        // Verifica se quem entrou é o jogador
        TimeTravel player = other.GetComponentInParent<TimeTravel>();

        if (player == null) return;

        // Verifica se é o jogador configurado
        if (timeTravel != null && player != timeTravel)
            return;

        Collect(player);
    }

    private void Collect(TimeTravel player)
    {
        collected = true;

        // Libera a viagem no tempo
        player.CollectFairy();

        // Ativa a fada
        if (fairy != null)
            fairy.SetActive(true);

        // Desativa o âmbar
        if (amber != null)
            amber.SetActive(false);

        Debug.Log("Fada libertada do âmbar!");
    }
}
