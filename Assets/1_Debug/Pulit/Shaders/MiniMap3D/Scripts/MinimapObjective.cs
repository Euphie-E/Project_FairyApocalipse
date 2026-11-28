using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1100)]
public class MinimapObjective : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jogador;
    [SerializeField] private Camera cameraMinimapa;
    [SerializeField] private RectTransform interfaceMinimapa;

    [Header("Configuracoes do Grupo")]
    [SerializeField] private bool objetivoAtivo = true;
    [SerializeField] private Color corObjetivo = Color.yellow;

    [Min(0.1f)]
    [SerializeField] private float alcanceMaximo = 50f;

    [Header("Indicadores de Borda")]
    [SerializeField] private bool mostrarIndicadorBorda = true;

    [Min(1f)]
    [SerializeField] private float tamanhoIndicador = 12f;

    [Min(0f)]
    [SerializeField] private float margemBorda = 20f;

    private class Objetivo
    {
        public Renderer renderer;
        public Image indicador;
    }

    private readonly List<Objetivo> objetivos =
        new List<Objetivo>();

    private MaterialPropertyBlock bloco;

    private Color ultimaCor;
    private bool corInicializada;

    private static readonly int CorID =
        Shader.PropertyToID("_CorObjetivo");


    private void Awake()
    {
        bloco = new MaterialPropertyBlock();

        RefreshRenderers();
    }


    private void LateUpdate()
    {
        if (!corInicializada || ultimaCor != corObjetivo)
            AtualizarCor();

        bool grupoVisivel =
            objetivoAtivo &&
            jogador != null &&
            cameraMinimapa != null &&
            cameraMinimapa.isActiveAndEnabled;

        foreach (Objetivo objetivo in objetivos)
        {
            if (objetivo.renderer == null)
            {
                OcultarIndicador(objetivo);
                continue;
            }

            if (!grupoVisivel ||
                !objetivo.renderer.gameObject.activeInHierarchy)
            {
                objetivo.renderer.enabled = false;
                OcultarIndicador(objetivo);
                continue;
            }

            // Posicao individual de cada mesh.
            Vector3 posicao =
                objetivo.renderer.bounds.center;

            float distanciaQuadrada =
                (posicao - jogador.position).sqrMagnitude;

            bool dentroAlcance =
                distanciaQuadrada <=
                alcanceMaximo * alcanceMaximo;

            objetivo.renderer.enabled = dentroAlcance;

            if (dentroAlcance)
                AtualizarIndicador(objetivo, posicao);
            else
                OcultarIndicador(objetivo);
        }
    }


    public void RefreshRenderers()
    {
        LimparIndicadores();

        objetivos.Clear();

        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            Objetivo objetivo = new Objetivo
            {
                renderer = r,
                indicador = CriarIndicador(r.gameObject.name)
            };

            objetivos.Add(objetivo);
        }

        corInicializada = false;
    }


    private Image CriarIndicador(string nome)
    {
        if (interfaceMinimapa == null)
            return null;

        GameObject obj = new GameObject(
            nome + "_Indicador",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        obj.transform.SetParent(
            interfaceMinimapa,
            false
        );

        Image imagem = obj.GetComponent<Image>();

        RectTransform rect = imagem.rectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta =
            Vector2.one * tamanhoIndicador;

        rect.localRotation =
            Quaternion.Euler(0f, 0f, 45f);

        imagem.color = corObjetivo;
        imagem.raycastTarget = false;
        imagem.enabled = false;

        return imagem;
    }


    private void AtualizarIndicador(
        Objetivo objetivo,
        Vector3 posicao)
    {
        if (objetivo.indicador == null)
            return;

        if (!mostrarIndicadorBorda ||
            interfaceMinimapa == null ||
            !interfaceMinimapa.gameObject.activeInHierarchy)
        {
            OcultarIndicador(objetivo);
            return;
        }

        Vector3 viewport =
            cameraMinimapa.WorldToViewportPoint(posicao);

        bool dentroCamera =
            viewport.x >= 0f &&
            viewport.x <= 1f &&
            viewport.y >= 0f &&
            viewport.y <= 1f &&
            viewport.z >= cameraMinimapa.nearClipPlane &&
            viewport.z <= cameraMinimapa.farClipPlane;

        objetivo.indicador.enabled = !dentroCamera;

        if (dentroCamera)
            return;

        Vector2 tamanho = interfaceMinimapa.rect.size;

        Vector2 direcao = new Vector2(
            (viewport.x - 0.5f) * tamanho.x,
            (viewport.y - 0.5f) * tamanho.y
        );

        if (direcao.sqrMagnitude < 0.0001f)
            direcao = Vector2.up;

        float limiteX = Mathf.Max(
            1f,
            tamanho.x * 0.5f -
            margemBorda -
            tamanhoIndicador
        );

        float limiteY = Mathf.Max(
            1f,
            tamanho.y * 0.5f -
            margemBorda -
            tamanhoIndicador
        );

        float fator = Mathf.Max(
            Mathf.Abs(direcao.x) / limiteX,
            Mathf.Abs(direcao.y) / limiteY
        );

        objetivo.indicador.rectTransform.anchoredPosition =
            direcao / Mathf.Max(0.0001f, fator);

        objetivo.indicador.rectTransform.sizeDelta =
            Vector2.one * tamanhoIndicador;
    }


    private void AtualizarCor()
    {
        if (bloco == null)
            bloco = new MaterialPropertyBlock();

        foreach (Objetivo objetivo in objetivos)
        {
            if (objetivo.renderer != null)
            {
                objetivo.renderer.GetPropertyBlock(bloco);

                bloco.SetColor(CorID, corObjetivo);

                objetivo.renderer.SetPropertyBlock(bloco);
            }

            if (objetivo.indicador != null)
                objetivo.indicador.color = corObjetivo;
        }

        ultimaCor = corObjetivo;
        corInicializada = true;
    }


    private void OcultarIndicador(Objetivo objetivo)
    {
        if (objetivo.indicador != null)
            objetivo.indicador.enabled = false;
    }


    private void OcultarTudo()
    {
        foreach (Objetivo objetivo in objetivos)
        {
            if (objetivo.renderer != null)
                objetivo.renderer.enabled = false;

            OcultarIndicador(objetivo);
        }
    }


    private void LimparIndicadores()
    {
        foreach (Objetivo objetivo in objetivos)
        {
            if (objetivo.indicador != null)
            {
                objetivo.indicador.enabled = false;

                Destroy(objetivo.indicador.gameObject);
            }
        }
    }


    public void DefinirCor(Color novaCor)
    {
        corObjetivo = novaCor;
    }


    public void DefinirObjetivoAtivo(bool ativo)
    {
        objetivoAtivo = ativo;

        if (!ativo)
            OcultarTudo();
    }


    public void ConcluirObjetivo()
    {
        DefinirObjetivoAtivo(false);
    }


    private void OnDisable()
    {
        OcultarTudo();
    }


    private void OnDestroy()
    {
        LimparIndicadores();
    }
}