
using UnityEngine;

public class PlantLeafGrowth : MonoBehaviour
{
    public enum ModoInicio
    {
        AntesDoGalho,
        DuranteOGalho
    }

    [Header("Referencias")]
    public PlantTrunkGrowth tronco;
    public PlantBranchGrowth galho;
    public Transform visualFolha;
    public Transform marcadorNoGalho;

    [Header("Inicio da Folha")]
    public ModoInicio modoInicio = ModoInicio.DuranteOGalho;

    [Range(0f, 0.3f)]
    public float antecipacaoTronco = 0.05f;

    [Range(0f, 1f)]
    public float inicioNoGalho = 0.5f;

    [Header("Crescimento")]
    [Min(0.1f)]
    public float duracao = 1.5f;

    public AnimationCurve curvaCrescimento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotacao")]
    public Vector3 rotacaoInicial = new Vector3(0, 0, 25);

    public AnimationCurve curvaRotacao =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Acompanhamento")]
    public bool seguirRotacaoDoMarcador = false;

    [Header("Collider")]
    [Min(0f)]
    public float tempoAtivarCollider = 1.5f;

    private MeshCollider colliderFolha;

    public float ProgressoFolha { get; private set; }

    private Vector3 escalaFinal;
    private Quaternion rotacaoFinal;

    private float tempo;
    private float ultimoProgressoTronco = -1f;
    private bool iniciou;

    private void Awake()
    {
        if (tronco == null || galho == null || visualFolha == null)
        {
            Debug.LogError(
                "Configure Tronco, Galho e Visual da Folha!",
                this
            );

            enabled = false;
            return;
        }

        escalaFinal = visualFolha.localScale;
        rotacaoFinal = visualFolha.localRotation;

        colliderFolha =
            visualFolha.GetComponentInChildren<MeshCollider>(true);

        ReiniciarFolha();
    }

    private void LateUpdate()
    {
        float progressoTronco = tronco.ProgressoCrescimento;

        // Detecta quando o tronco reinicia
        if (ultimoProgressoTronco >= 0f &&
            progressoTronco < ultimoProgressoTronco - 0.001f)
        {
            ReiniciarFolha();
        }

        ultimoProgressoTronco = progressoTronco;

        // Acompanha o marcador do galho
        if (marcadorNoGalho != null)
        {
            transform.position = marcadorNoGalho.position;

            if (seguirRotacaoDoMarcador)
                transform.rotation = marcadorNoGalho.rotation;
        }

        // Aguarda o momento de iniciar
        if (!iniciou)
        {
            bool podeIniciar = false;

            if (modoInicio == ModoInicio.AntesDoGalho)
            {
                float pontoInicio = Mathf.Max(
                    0f,
                    galho.inicioNoTronco - antecipacaoTronco
                );

                podeIniciar = progressoTronco >= pontoInicio;
            }
            else
            {
                podeIniciar =
                    progressoTronco >= galho.inicioNoTronco &&
                    galho.ProgressoGalho >= inicioNoGalho;
            }

            if (!podeIniciar)
                return;

            iniciou = true;
            tempo = 0f;
        }

        tempo += Time.deltaTime;

        float progresso = Mathf.Clamp01(
            tempo / Mathf.Max(0.1f, duracao)
        );

        AplicarCrescimento(progresso);

        // Ativa o collider pelo tempo configurado
        if (colliderFolha != null)
        {
            colliderFolha.enabled =
                tempo >= tempoAtivarCollider;
        }
    }

    private void AplicarCrescimento(float progresso)
    {
        float crescimento = Mathf.Clamp01(
            curvaCrescimento.Evaluate(progresso)
        );

        float rotacao = Mathf.Clamp01(
            curvaRotacao.Evaluate(progresso)
        );

        ProgressoFolha = crescimento;

        // Escala da folha
        visualFolha.localScale = Vector3.Lerp(
            Vector3.zero,
            escalaFinal,
            crescimento
        );

        // Rotacao da folha
        Vector3 rotacaoExtra =
            rotacaoInicial * (1f - rotacao);

        visualFolha.localRotation =
            rotacaoFinal * Quaternion.Euler(rotacaoExtra);
    }

    private void ReiniciarFolha()
    {
        tempo = 0f;
        iniciou = false;

        AplicarCrescimento(0f);

        // Desativa collider ao reiniciar
        if (colliderFolha != null)
            colliderFolha.enabled = false;
    }
}
