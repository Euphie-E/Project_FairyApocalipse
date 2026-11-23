
using UnityEngine;

public class FogueiraVFX : MonoBehaviour
{
    [Header("Particulas da Fogueira")]
    [SerializeField] private ParticleSystem[] particulas;

    [Header("Iluminacao")]
    [SerializeField] private Light luzFogueira;
    [SerializeField] private float intensidadeBase = 2f;
    [SerializeField, Range(0f, 1f)]
    private float variacaoLuz = 0.15f;
    [SerializeField] private float velocidadeOscilacao = 2f;
    [SerializeField] private float velocidadeTransicao = 4f;

    [Header("Audio")]
    [SerializeField] private string nomeAudio = "Fogueira";

    [Header("Estado Inicial")]
    [SerializeField] private bool comecaAcesa = true;

    private bool acesa;
    private float sementeRuido;

    private void Awake()
    {
        sementeRuido = Mathf.Abs(GetInstanceID() * 0.13f);
        acesa = comecaAcesa;

        foreach (ParticleSystem ps in particulas)
        {
            if (ps == null) continue;

            if (acesa)
                ps.Play();
            else
                ps.Stop(true,
                    ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (luzFogueira != null)
        {
            luzFogueira.enabled = acesa;
            luzFogueira.intensity = acesa ? intensidadeBase : 0f;
        }
    }

    private void Start()
    {
        if (acesa)
            GCamarada.AudioManager.I?.PlayClip(nomeAudio, true);
    }

    private void Update()
    {
        if (luzFogueira == null) return;

        float intensidadeAlvo = 0f;

        if (acesa)
        {
            float ruido = Mathf.PerlinNoise(
                sementeRuido,
                Time.time * velocidadeOscilacao
            );

            float variacao = Mathf.Lerp(
                -variacaoLuz,
                variacaoLuz,
                ruido
            );

            intensidadeAlvo = intensidadeBase * (1f + variacao);
            luzFogueira.enabled = true;
        }

        luzFogueira.intensity = Mathf.MoveTowards(
            luzFogueira.intensity,
            intensidadeAlvo,
            velocidadeTransicao * Time.deltaTime
        );

        if (!acesa && luzFogueira.intensity <= 0.01f)
        {
            luzFogueira.intensity = 0f;
            luzFogueira.enabled = false;
        }
    }

    public void AlternarFogueira()
    {
        DefinirFogueira(!acesa);
    }

    public void DefinirFogueira(bool ligar)
    {
        if (acesa == ligar) return;

        acesa = ligar;

        foreach (ParticleSystem ps in particulas)
        {
            if (ps == null) continue;

            if (acesa)
                ps.Play();
            else
                ps.Stop(true,
                    ParticleSystemStopBehavior.StopEmitting);
        }

        if (acesa)
            GCamarada.AudioManager.I?.PlayClip(nomeAudio, true);
        else
            GCamarada.AudioManager.I?.StopClip(nomeAudio);
    }
}
