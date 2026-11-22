
using UnityEngine;

public class BossImpactEvents : MonoBehaviour
{
    [Header("VFX dos 5 Impactos")]
    [SerializeField] private GroundImpactVFX[] impactos = new GroundImpactVFX[5];

    // Chamado pelo Animation Event
    public void ExecutarImpacto(int indice)
    {
        if (indice < 0 || indice >= impactos.Length)
            return;

        if (impactos[indice] != null)
            impactos[indice].Executar();
    }
}
