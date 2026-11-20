
using UnityEngine;

public class StormVFX : MonoBehaviour
{
    public enum Intensidade
    {
        Fraca,
        Forte,
        Temporal
    }

    [Header("Chuva")]
    [SerializeField] private ParticleSystem psRainMain;
    [SerializeField] private ParticleSystem psRainClose;
    [SerializeField] private ParticleSystem psGroundSplashes;
    [SerializeField] private ParticleSystem psWaterRipples;

    [Header("Atmosfera")]
    [SerializeField] private ParticleSystem psGroundMist;
    [SerializeField] private ParticleSystem psWindSpray;
    [SerializeField] private ParticleSystem psStormClouds;

    [Header("Relampagos")]
    [SerializeField] private LightningVFX lightning;

    [Header("Posicionamento")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraReferencia;
    [SerializeField] private bool seguirPlayer = true;

    [Header("Configuracoes")]
    [SerializeField] private bool comecaAtiva = true;
    [SerializeField]
    private Intensidade intensidadeInicial =
        Intensidade.Temporal;

    private ParticleSystem[] sistemas;
    private float[] emissoesOriginais;
    private float alturaRainClose;

    private bool ativa;
    private Intensidade intensidadeAtual;

    private void Awake()
    {
        sistemas = new ParticleSystem[]
        {
            psRainMain,
            psRainClose,
            psGroundSplashes,
            psWaterRipples,
            psGroundMist,
            psWindSpray,
            psStormClouds
        };

        emissoesOriginais = new float[sistemas.Length];

        for (int i = 0; i < sistemas.Length; i++)
        {
            if (sistemas[i] == null) continue;

            emissoesOriginais[i] =
                sistemas[i].emission.rateOverTimeMultiplier;
        }

        if (psRainClose != null)
            alturaRainClose = psRainClose.transform.localPosition.y;
    }

    private void Start()
    {
        DefinirIntensidade(intensidadeInicial);
        DefinirAtiva(comecaAtiva);
    }

    private void LateUpdate()
    {
        if (!seguirPlayer || player == null)
            return;

        // Move a area da tempestade com o jogador.
        // Mantem a altura da base fixa.
        transform.position = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );

        // Centraliza a chuva proxima na camera.
        if (cameraReferencia != null && psRainClose != null)
        {
            psRainClose.transform.localPosition = new Vector3(
                cameraReferencia.position.x - transform.position.x,
                alturaRainClose,
                cameraReferencia.position.z - transform.position.z
            );
        }
    }

    public void AlternarTempestade()
    {
        DefinirAtiva(!ativa);
    }


    public void DefinirAtiva(bool ligar)
    {
        ativa = ligar;

        // Tempestade ligada = Fog ligado
        // Tempestade desligada = Fog desligado
        RenderSettings.fog = ativa;

        // Controla os Particle Systems
        foreach (ParticleSystem ps in sistemas)
        {
            if (ps == null) continue;

            if (ativa)
            {
                if (!ps.isEmitting)
                    ps.Play(false);
            }
            else
            {
                ps.Stop(
                    false,
                    ParticleSystemStopBehavior.StopEmitting
                );
            }
        }

        // Controla os relampagos
        if (lightning != null)
        {
            lightning.gameObject.SetActive(ativa);
        }
    }

    private void OnDisable()
    {
        RenderSettings.fog = false;
    }

    public void DefinirIntensidade(Intensidade nova)
    {
        intensidadeAtual = nova;

        float fator = 1f;

        switch (intensidadeAtual)
        {
            case Intensidade.Fraca:
                fator = 0.25f;
                break;

            case Intensidade.Forte:
                fator = 0.65f;
                break;

            case Intensidade.Temporal:
                fator = 1f;
                break;
        }

        float[] fatores =
        {
            fator,                 // RainMain
            fator,                 // RainClose
            fator,                 // Splashes
            fator,                 // Ripples
            0.4f + fator * 0.6f, // Mist
            fator * fator,         // WindSpray
            0.65f + fator * 0.35f // Clouds
        };

        for (int i = 0; i < sistemas.Length; i++)
        {
            if (sistemas[i] == null) continue;

            var emission = sistemas[i].emission;

            emission.rateOverTimeMultiplier =
                emissoesOriginais[i] * fatores[i];
        }
    }

    public void ChuvaFraca()
    {
        DefinirIntensidade(Intensidade.Fraca);
    }

    public void ChuvaForte()
    {
        DefinirIntensidade(Intensidade.Forte);
    }

    public void Temporal()
    {
        DefinirIntensidade(Intensidade.Temporal);
    }
}
