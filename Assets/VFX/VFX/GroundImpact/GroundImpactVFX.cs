
using UnityEngine;

public class GroundImpactVFX : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem psRocks;
    [SerializeField] private ParticleSystem psDust;
    [SerializeField] private ParticleSystem psShockwave;
    [SerializeField] private ParticleSystem psFragments;

    [Header("Configuracao do Impacto")]
    [SerializeField] private float raio = 2f;
    [SerializeField] private float tamanhoPedras = 1f;
    [SerializeField] private float tempoDestruicao = 3f;

    private bool executado = false;

    public void Executar()
    {
        if (executado) return;
        executado = true;

        ConfigurarImpacto();

        ParticleSystem[] sistemas =
        {
            psRocks, psDust, psShockwave, psFragments
        };

        foreach (ParticleSystem ps in sistemas)
        {
            if (ps == null) continue;

            ps.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            ps.Play(true);
        }

        Destroy(gameObject, tempoDestruicao);
    }

    private void ConfigurarImpacto()
    {
        if (psRocks != null)
        {
            var shape = psRocks.shape;
            shape.radius = raio;

            var main = psRocks.main;

            if (main.startSize3D)
            {
                main.startSizeXMultiplier *= tamanhoPedras;
                main.startSizeYMultiplier *= tamanhoPedras;
                main.startSizeZMultiplier *= tamanhoPedras;
            }
            else
            {
                main.startSizeMultiplier *= tamanhoPedras;
            }
        }

        if (psDust != null)
        {
            var shape = psDust.shape;
            shape.radius = raio * 0.5f;
        }

        if (psShockwave != null)
        {
            var velocidade = psShockwave.velocityOverLifetime;
            velocidade.enabled = true;
            velocidade.radial =
                new ParticleSystem.MinMaxCurve(raio / 0.45f);
        }

        if (psFragments != null)
        {
            var shape = psFragments.shape;
            shape.radius = raio * 0.4f;
        }
    }
}
