
using System.Collections.Generic;
using UnityEngine;

public class PlantTrunkSequence : MonoBehaviour
{
    [Header("Troncos em ordem de crescimento")]
    public List<PlantTrunkGrowth> troncos =
        new List<PlantTrunkGrowth>();

    [Header("Inicializacao")]
    public bool iniciarAutomaticamente = false;

    private int indiceAtual = -1;
    private bool sequenciaAtiva;

    private void Start()
    {
        // Todos os troncos aguardam escondidos.
        foreach (PlantTrunkGrowth tronco in troncos)
        {
            if (tronco == null)
                continue;

            tronco.DefinirCrescimento(0f);
        }

        if (iniciarAutomaticamente)
            IniciarPlantio();
    }

    public void IniciarPlantio()
    {
        if (sequenciaAtiva || troncos.Count == 0)
            return;

        foreach (PlantTrunkGrowth tronco in troncos)
        {
            if (tronco == null)
            {
                Debug.LogError(
                    "Existe um tronco vazio na lista!",
                    this
                );
                return;
            }
        }

        indiceAtual = 0;
        sequenciaAtiva = true;

        // Comeca pelo primeiro tronco.
        troncos[indiceAtual].IniciarCrescimento();
    }

    private void LateUpdate()
    {
        if (!sequenciaAtiva)
            return;

        PlantTrunkGrowth troncoAtual =
            troncos[indiceAtual];

        // Aguarda o crescimento terminar.
        if (troncoAtual.ProgressoCrescimento < 0.999f)
            return;

        // Avanca para o proximo tronco.
        indiceAtual++;

        if (indiceAtual >= troncos.Count)
        {
            sequenciaAtiva = false;
            return;
        }

        troncos[indiceAtual].IniciarCrescimento();
    }
}
