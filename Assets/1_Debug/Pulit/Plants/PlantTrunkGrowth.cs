using UnityEngine;

public class PlantTrunkGrowth : MonoBehaviour
{
    [Header("Tronco")]
    [SerializeField] private MeshRenderer troncoRenderer;

    [Header("Crescimento")]
    [Min(0.1f)]
    public float duracao = 5f;

    public AnimationCurve curvaCrescimento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool iniciarAutomaticamente = true;

    [Header("Rotacao")]
    public Transform rotacaoPivot;

    [Tooltip("Rotacao adicional no inicio do crescimento")]
    public Vector3 rotacaoInicial =
        new Vector3(0f, 60f, 10f);

    public AnimationCurve curvaRotacao =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Quaternion rotacaoFinal;

    private MaterialPropertyBlock propriedades;

    private static readonly int GrowthHeight =
        Shader.PropertyToID("_GrowthHeight");

    public float ProgressoCrescimento { get; private set; }

    private float alturaMinima;
    private float alturaMaxima;
    private float tempo;

    private bool crescendo;

    private void Awake()
    {
        if (troncoRenderer == null)
            troncoRenderer =
                GetComponentInChildren<MeshRenderer>();

        if (troncoRenderer == null)
        {
            Debug.LogError("Tronco nao encontrado!");
            enabled = false;
            return;
        }

        MeshFilter filtro =
            troncoRenderer.GetComponent<MeshFilter>();

        if (filtro == null || filtro.sharedMesh == null)
        {
            Debug.LogError("Mesh do tronco nao encontrada!");
            enabled = false;
            return;
        }

        Bounds bounds = filtro.sharedMesh.bounds;

        alturaMinima = bounds.min.y - 0.01f;
        alturaMaxima = bounds.max.y + 0.01f;

        propriedades = new MaterialPropertyBlock();

        if (rotacaoPivot != null)
            rotacaoFinal = rotacaoPivot.localRotation;

        float progressoInicial =
            iniciarAutomaticamente ? 0f : 1f;

        DefinirCrescimento(progressoInicial);
        AplicarRotacao(progressoInicial);
    }

    private void OnEnable()
    {
        if (iniciarAutomaticamente)
            IniciarCrescimento();
    }

    private void Update()
    {
        if (!crescendo)
            return;

        tempo += Time.deltaTime;

        float progresso = Mathf.Clamp01(
            tempo / duracao
        );

        float crescimento = Mathf.Clamp01(
            curvaCrescimento.Evaluate(progresso)
        );

        DefinirCrescimento(crescimento);
        AplicarRotacao(progresso);

        if (progresso >= 1f)
        {
            crescendo = false;

            DefinirCrescimento(1f);
            AplicarRotacao(1f);
        }
    }

    [ContextMenu("Reiniciar Crescimento")]
    public void IniciarCrescimento()
    {
        if (!Application.isPlaying)
            return;

        tempo = 0f;
        crescendo = true;

        DefinirCrescimento(0f);
        AplicarRotacao(0f);
    }

    public void DefinirCrescimento(float valor)
    {
        if (troncoRenderer == null ||
            propriedades == null)
            return;

        valor = Mathf.Clamp01(valor);
        ProgressoCrescimento = valor;

        float altura = Mathf.Lerp(
            alturaMinima,
            alturaMaxima,
            valor
        );

        troncoRenderer.GetPropertyBlock(propriedades);

        propriedades.SetFloat(
            GrowthHeight,
            altura
        );

        troncoRenderer.SetPropertyBlock(propriedades);
    }

    private void AplicarRotacao(float progresso)
    {
        if (rotacaoPivot == null)
            return;

        float valor = Mathf.Clamp01(
            curvaRotacao.Evaluate(progresso)
        );

        Vector3 rotacaoRestante =
            rotacaoInicial * (1f - valor);

        rotacaoPivot.localRotation =
            rotacaoFinal *
            Quaternion.Euler(rotacaoRestante);
    }
}
