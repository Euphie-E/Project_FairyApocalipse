
using UnityEngine;
using System.Collections.Generic;

public class SpawnerGaivotas : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Camera cameraReferencia;
    public List<GameObject> prefabsGaivota = new();

    [Header("Quantidade de Bandos")]
    [Min(1)] public int maximoBandos = 5;
    [Min(1)] public int minimoPorBando = 3;
    [Min(1)] public int maximoPorBando = 5;

    [Header("Area de Spawn")]
    public float raioMinimoSpawn = 60f;
    public float raioMaximoSpawn = 100f;
    public float raioDespawn = 170f;

    [Header("Altura do Voo")]
    public float alturaMinima = 25f;
    public float alturaMaxima = 45f;

    [Header("Formacao dos Bandos")]
    public float dispersaoBando = 7f;

    [Header("Otimizacao")]
    [Min(0.2f)]
    public float intervaloAtualizacao = 2f;

    public bool evitarSpawnNaCamera = true;

    [Range(0f, 0.5f)]
    public float margemTela = 0.15f;

    private class Bando
    {
        public List<GameObject> gaivotas = new();
    }

    private readonly List<Bando> bandosAtivos = new();

    private void Start()
    {
        if (player == null)
            player = transform;

        if (cameraReferencia == null)
            cameraReferencia = Camera.main;

        if (prefabsGaivota == null ||
            prefabsGaivota.Count == 0)
        {
            Debug.LogWarning(
                "SpawnerGaivotas: nenhum prefab configurado."
            );
            enabled = false;
            return;
        }

        InvokeRepeating(
            nameof(AtualizarBandos),
            0f,
            Mathf.Max(0.2f, intervaloAtualizacao)
        );
    }

    private void AtualizarBandos()
    {
        if (player == null)
            return;

        // Remove bandos que ficaram distantes.
        for (int i = bandosAtivos.Count - 1; i >= 0; i--)
        {
            Bando bando = bandosAtivos[i];

            if (BandoEstaDistante(bando))
            {
                DestruirBando(bando);
                bandosAtivos.RemoveAt(i);
            }
        }

        // Cria no maximo um novo bando por atualizacao.
        if (bandosAtivos.Count < maximoBandos)
        {
            CriarBando();
        }
    }

    private bool BandoEstaDistante(Bando bando)
    {
        float distanciaLimite =
            raioDespawn * raioDespawn;

        foreach (GameObject gaivota in bando.gaivotas)
        {
            if (gaivota == null)
                continue;

            Vector3 diferenca =
                gaivota.transform.position - player.position;

            // Considera a distancia horizontal.
            diferenca.y = 0f;

            if (diferenca.sqrMagnitude <= distanciaLimite)
                return false;
        }

        // Todas estao distantes ou foram destruidas.
        return true;
    }

    private void CriarBando()
    {
        Vector3 centroBando = Vector3.zero;
        bool encontrouPosicao = false;

        // Tenta encontrar um ponto fora da camera.
        for (int tentativa = 0; tentativa < 20; tentativa++)
        {
            float angulo = Random.Range(
                0f, Mathf.PI * 2f
            );

            float distancia = Random.Range(
                raioMinimoSpawn,
                raioMaximoSpawn
            );

            Vector3 deslocamento = new Vector3(
                Mathf.Cos(angulo) * distancia,
                Random.Range(alturaMinima, alturaMaxima),
                Mathf.Sin(angulo) * distancia
            );

            centroBando = player.position + deslocamento;

            if (evitarSpawnNaCamera &&
                cameraReferencia != null &&
                EstaNaCamera(centroBando))
            {
                continue;
            }

            encontrouPosicao = true;
            break;
        }

        if (!encontrouPosicao)
            return;

        Bando novoBando = new Bando();

        int quantidade = Random.Range(
            minimoPorBando,
            maximoPorBando + 1
        );

        for (int i = 0; i < quantidade; i++)
        {
            GameObject prefab = ObterPrefabAleatorio();

            if (prefab == null)
                continue;

            Vector2 offset = Random.insideUnitCircle *
                             dispersaoBando;

            Vector3 posicao = centroBando + new Vector3(
                offset.x,
                Random.Range(-2f, 2f),
                offset.y
            );

            GameObject gaivota = Instantiate(
                prefab,
                posicao,
                Quaternion.Euler(
                    0f,
                    Random.Range(0f, 360f),
                    0f
                )
            );

            MovimentoGaivota movimento =
                gaivota.GetComponent<MovimentoGaivota>();

            if (movimento != null)
                movimento.DefinirCentro(centroBando);

            novoBando.gaivotas.Add(gaivota);
        }

        if (novoBando.gaivotas.Count > 0)
            bandosAtivos.Add(novoBando);
    }

    private bool EstaNaCamera(Vector3 posicao)
    {
        Vector3 viewport =
            cameraReferencia.WorldToViewportPoint(posicao);

        if (viewport.z <= 0f)
            return false;

        return viewport.x >= -margemTela &&
               viewport.x <= 1f + margemTela &&
               viewport.y >= -margemTela &&
               viewport.y <= 1f + margemTela;
    }

    private GameObject ObterPrefabAleatorio()
    {
        if (prefabsGaivota.Count == 0)
            return null;

        int inicio = Random.Range(0, prefabsGaivota.Count);

        for (int i = 0; i < prefabsGaivota.Count; i++)
        {
            GameObject prefab = prefabsGaivota[
                (inicio + i) % prefabsGaivota.Count
            ];

            if (prefab != null)
                return prefab;
        }

        return null;
    }

    private void DestruirBando(Bando bando)
    {
        foreach (GameObject gaivota in bando.gaivotas)
        {
            if (gaivota != null)
                Destroy(gaivota);
        }

        bando.gaivotas.Clear();
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(AtualizarBandos));

        foreach (Bando bando in bandosAtivos)
            DestruirBando(bando);

        bandosAtivos.Clear();
    }
}
