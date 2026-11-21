
using UnityEngine;

public class MovimentoGaivota : MonoBehaviour
{
    [Header("Movimento Circular")]
    public float raioVoo = 15f;
    public float velocidadeAngular = 25f;

    [Header("Velocidade")]
    public float velocidadeVooReto = 6f;
    public float velocidadeRotacao = 3f;

    [Header("Voo Reto")]
    public float tempoVooReto = 3f;

    [Range(0f, 1f)]
    public float chanceVooRetoPorSegundo = 0.3f;

    [Header("Limites")]
    public float distanciaMaxima = 35f;

    private Vector3 centro;
    private float anguloAtual;
    private float alturaFixaY;

    private float tempoRetoRestante;
    private bool voandoReto;
    private Vector3 direcaoVooReto;

    private float sentidoRotacao;

    private void Awake()
    {
        centro = transform.position;
        alturaFixaY = transform.position.y;

        anguloAtual = Random.Range(0f, 360f);

        sentidoRotacao = Random.value < 0.5f
            ? -1f
            : 1f;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        Vector3 posicao = transform.position;
        posicao.y = alturaFixaY;

        transform.position = posicao;

        Vector3 direcao = Vector3.zero;

        Vector3 diferencaCentro = centro - posicao;
        diferencaCentro.y = 0f;

        float distanciaCentro = diferencaCentro.magnitude;

        // Retorna ao centro quando ultrapassa o limite.
        if (distanciaCentro > distanciaMaxima)
        {
            voandoReto = false;
            direcao = diferencaCentro.normalized;
        }
        else if (voandoReto)
        {
            direcao = direcaoVooReto;

            tempoRetoRestante -= delta;

            if (tempoRetoRestante <= 0f)
                voandoReto = false;
        }
        else
        {
            anguloAtual +=
                velocidadeAngular * sentidoRotacao * delta;

            float rad = anguloAtual * Mathf.Deg2Rad;

            Vector3 pontoAlvo = centro + new Vector3(
                Mathf.Cos(rad) * raioVoo,
                0f,
                Mathf.Sin(rad) * raioVoo
            );

            direcao = pontoAlvo - posicao;
            direcao.y = 0f;

            if (direcao.sqrMagnitude > 0.0001f)
                direcao.Normalize();

            // Probabilidade por segundo, independente do FPS.
            if (Random.value <
                chanceVooRetoPorSegundo * delta)
            {
                voandoReto = true;
                tempoRetoRestante = tempoVooReto;

                direcaoVooReto = transform.forward;
                direcaoVooReto.y = 0f;

                if (direcaoVooReto.sqrMagnitude < 0.0001f)
                    direcaoVooReto = direcao;

                direcaoVooReto.Normalize();
            }
        }

        if (direcao.sqrMagnitude < 0.0001f)
            return;

        // Deslocamento.
        transform.position +=
            direcao * velocidadeVooReto * delta;

        // Rotacao suave para a direcao do movimento.
        Quaternion rotacaoAlvo =
            Quaternion.LookRotation(direcao, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacaoAlvo,
            velocidadeRotacao * delta
        );
    }

    public void DefinirCentro(Vector3 novoCentro)
    {
        centro = new Vector3(
            novoCentro.x,
            alturaFixaY,
            novoCentro.z
        );
    }
}
