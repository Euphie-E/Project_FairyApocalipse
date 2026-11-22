
using UnityEngine;

public class GroundImpactVFX : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem psRocks;
    [SerializeField] private ParticleSystem psDust;
    [SerializeField] private ParticleSystem psShockwave;
    [SerializeField] private ParticleSystem psFragments;

    public void Executar()
    {
        ParticleSystem[] sistemas =
        {
            psRocks, psDust, psShockwave, psFragments
        };

        foreach (ParticleSystem ps in sistemas)
        {
            if (ps == null) continue;

            // Limpa as partículas anteriores
            ps.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            // Reproduz novamente
            ps.Play(true);
        }
    }
}
