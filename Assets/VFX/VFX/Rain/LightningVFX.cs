
using System.Collections;
using UnityEngine;

public class LightningVFX : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer lightningCore;
    [SerializeField] private LineRenderer lightningGlow;

    [Header("Configuracao do Raio")]
    [SerializeField] private float alturaNuvens = 12f;
    [SerializeField] private float areaRaio = 7f;
    [SerializeField] private float alcanceLateral = 3f;
    [SerializeField] private int segmentos = 18;
    [SerializeField] private float irregularidade = 0.8f;

    [Header("Clarão")]
    [SerializeField] private Light luzRelampago;
    [SerializeField] private float intensidadeLuz = 8f;

    [Header("Teste")]
    [SerializeField] private bool testarComTecla = true;


    [Header("Relampagos Automaticos")]
    [SerializeField] private bool raiosAutomaticos = true;
    [SerializeField] private float intervaloMinimo = 3f;
    [SerializeField] private float intervaloMaximo = 7f;


    [Header("Impacto Eletrico")]
    [SerializeField] private ParticleSystem psLightningSparks;


    private Coroutine rotinaAutomatica;


    private Coroutine rotinaRaio;

    private void Awake()
    {
        MostrarRaio(false);
    }

    private void Update()
    {
        if (testarComTecla && Input.GetKeyDown(KeyCode.Alpha0))
        {
            DispararRaio();
        }
    }


    private void Start()
    {
        DefinirRaiosAutomaticos(raiosAutomaticos);
    }

    private IEnumerator GerarRaiosAutomaticamente()
    {
        while (true)
        {
            float minimo = Mathf.Max(0.1f, intervaloMinimo);
            float maximo = Mathf.Max(minimo, intervaloMaximo);

            float espera = Random.Range(minimo, maximo);

            yield return new WaitForSeconds(espera);

            DispararRaio();
        }
    }

    public void DefinirRaiosAutomaticos(bool ativar)
    {
        raiosAutomaticos = ativar;

        if (rotinaAutomatica != null)
        {
            StopCoroutine(rotinaAutomatica);
            rotinaAutomatica = null;
        }

        if (ativar && isActiveAndEnabled)
        {
            rotinaAutomatica =
                StartCoroutine(GerarRaiosAutomaticamente());
        }
        else if (!ativar)
        {
            if (rotinaRaio != null)
            {
                StopCoroutine(rotinaRaio);
                rotinaRaio = null;
            }

            MostrarRaio(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        rotinaAutomatica = null;
        rotinaRaio = null;

        MostrarRaio(false);
    }

    private void OnEnable()
    {
        if (raiosAutomaticos)
            DefinirRaiosAutomaticos(true);
    }


    public void DispararRaio()
    {
        if (lightningCore == null || lightningGlow == null)
            return;

        if (rotinaRaio != null)
            StopCoroutine(rotinaRaio);

        Vector3 centro = transform.position;

       
        Vector3 inicio = centro + new Vector3(
            Random.Range(-areaRaio, areaRaio),
            alturaNuvens,
            Random.Range(-areaRaio, areaRaio)
        );

        
        Vector3 fim = new Vector3(
            inicio.x + Random.Range(-alcanceLateral, alcanceLateral),
            centro.y + 0.05f,
            inicio.z + Random.Range(-alcanceLateral, alcanceLateral)
        );

        GerarCaminho(inicio, fim);

        if (psLightningSparks != null)
        {
        
            psLightningSparks.transform.position = fim;

            
            psLightningSparks.Stop(
                false,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            
            psLightningSparks.Play(false);
        }


        if (luzRelampago != null)
        {
            luzRelampago.transform.position =
                Vector3.Lerp(inicio, fim, 0.5f);

            luzRelampago.intensity = intensidadeLuz;
        }

        rotinaRaio = StartCoroutine(AnimarRaio());
    }

    private void GerarCaminho(Vector3 inicio, Vector3 fim)
    {
        int quantidade = Mathf.Max(2, segmentos);

        Vector3[] pontos = new Vector3[quantidade + 1];

        for (int i = 0; i <= quantidade; i++)
        {
            float t = (float)i / quantidade;

            Vector3 posicao = Vector3.Lerp(inicio, fim, t);

            if (i > 0 && i < quantidade)
            {
                posicao += new Vector3(
                    Random.Range(-irregularidade, irregularidade),
                    0f,
                    Random.Range(-irregularidade, irregularidade)
                );
            }

            pontos[i] = posicao;
        }

        lightningCore.positionCount = pontos.Length;
        lightningGlow.positionCount = pontos.Length;

        lightningCore.SetPositions(pontos);
        lightningGlow.SetPositions(pontos);
    }

    private IEnumerator AnimarRaio()
    {
        MostrarRaio(true);

        yield return new WaitForSeconds(0.06f);

        MostrarRaio(false);

        yield return new WaitForSeconds(0.035f);

        MostrarRaio(true);

        yield return new WaitForSeconds(0.1f);

        MostrarRaio(false);

        rotinaRaio = null;
    }

    private void MostrarRaio(bool mostrar)
    {
        if (lightningCore != null)
            lightningCore.enabled = mostrar;

        if (lightningGlow != null)
            lightningGlow.enabled = mostrar;

        if (luzRelampago != null)
            luzRelampago.enabled = mostrar;
    }
}
