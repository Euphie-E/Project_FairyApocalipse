
using UnityEngine;

public class PlantWobble : MonoBehaviour
{
    [Header("Balanco")]
    public float angulo = 5f;
    public float velocidade = 3f;
    public float amortecimento = 3f;
    public float duracao = 1.5f;

    [Header("Direcao")]
    public Vector3 eixo = Vector3.forward;

    private Quaternion rotacaoOriginal;
    private float tempo;
    private bool balancando;

    private void Awake()
    {
        rotacaoOriginal = transform.localRotation;
    }

    [ContextMenu("Testar Balanco")]
    public void Balancar()
    {
        tempo = 0f;
        balancando = true;
    }

    private void Update()
    {
        if (!balancando)
            return;

        tempo += Time.deltaTime;

        float forca = Mathf.Exp(
            -amortecimento * tempo
        );

        float oscilacao = Mathf.Sin(
            tempo * velocidade * Mathf.PI * 2f
        );

        float rotacao = oscilacao * angulo * forca;

        transform.localRotation =
            rotacaoOriginal *
            Quaternion.AngleAxis(rotacao, eixo.normalized);

        if (tempo >= duracao)
        {
            transform.localRotation = rotacaoOriginal;
            balancando = false;
        }
    }
}
