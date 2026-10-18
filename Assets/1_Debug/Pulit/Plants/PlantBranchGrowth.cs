using UnityEngine;

public class PlantBranchGrowth : MonoBehaviour
{
    [Header("Referencias")]
    public PlantTrunkGrowth tronco;
    public Transform visual;

    [Header("Ativacao")]
    [Range(0f, 1f)]
    public float inicioNoTronco = 0.5f;

    [Header("Crescimento")]
    [Min(0.1f)]
    public float duracao = 2f;

    public AnimationCurve curvaCrescimento =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotacao")]
    public Vector3 rotacaoInicial =
        new Vector3(0f, 0f, 30f);

    private Vector3 escalaFinal;
    private Quaternion rotacaoFinal;

    private float tempo;
    private bool iniciou;

    private void Awake()
    {
        if (tronco == null || visual == null)
        {
            Debug.LogError(
                "Configure o Tronco e o Visual do galho!",
                this
            );

            enabled = false;
            return;
        }

        escalaFinal = visual.localScale;
        rotacaoFinal = visual.localRotation;

        AplicarCrescimento(0f);
    }

    private void Update()
    {
        // Espera o tronco chegar ao ponto desejado
        if (tronco.ProgressoCrescimento < inicioNoTronco)
        {
            // Permite reiniciar junto com o tronco
            if (iniciou)
            {
                iniciou = false;
                tempo = 0f;
                AplicarCrescimento(0f);
            }

            return;
        }

        if (!iniciou)
        {
            iniciou = true;
            tempo = 0f;
        }

        tempo += Time.deltaTime;

        float progresso = Mathf.Clamp01(
            tempo / duracao
        );

        AplicarCrescimento(progresso);
    }

    private void AplicarCrescimento(float progresso)
    {
        float valor = Mathf.Clamp01(
            curvaCrescimento.Evaluate(progresso)
        );

        // Cresce ate a escala original
        visual.localScale = Vector3.Lerp(
            Vector3.zero,
            escalaFinal,
            valor
        );

        // Gira ate a rotacao original
        Vector3 rotacaoExtra =
            rotacaoInicial * (1f - valor);

        visual.localRotation =
            rotacaoFinal * Quaternion.Euler(rotacaoExtra);
    }
}
