
using UnityEngine;

public class TrashExplosionVFX : MonoBehaviour
{
    [Header("Particle Systems")]
   
    [SerializeField] private ParticleSystem psDust;
    [SerializeField] private ParticleSystem psShockwave;
    [SerializeField] private ParticleSystem psFragments;

    public void Executar()
    {
        ParticleSystem[] sistemas =
        {
            psDust, psShockwave, psFragments
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
