
using UnityEngine;

public class FirefliesVFX : MonoBehaviour
{
    [Header("Particulas")]
    [SerializeField] private ParticleSystem psFireflies;
    [SerializeField] private ParticleSystem psMagicDust;

    [Header("Configuracoes")]
    [SerializeField] private bool comecaAtivo = true;

    private bool ativo;

    private void Awake()
    {
        ativo = comecaAtivo;

        AtualizarParticulas(true);
    }

    public void Alternar()
    {
        DefinirAtivo(!ativo);
    }

    public void DefinirAtivo(bool ligar)
    {
        ativo = ligar;
        AtualizarParticulas(false);
    }

    private void AtualizarParticulas(bool imediato)
    {
        ControlarParticula(psFireflies, imediato);
        ControlarParticula(psMagicDust, imediato);
    }

    private void ControlarParticula(
        ParticleSystem ps, bool imediato)
    {
        if (ps == null) return;

        if (ativo)
        {
            ps.Play(false);
        }
        else
        {
            ps.Stop(
                false,
                imediato
                    ? ParticleSystemStopBehavior.StopEmittingAndClear
                    : ParticleSystemStopBehavior.StopEmitting
            );
        }
    }
}
