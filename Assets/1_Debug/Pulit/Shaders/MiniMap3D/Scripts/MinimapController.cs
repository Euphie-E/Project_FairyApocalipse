using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[DefaultExecutionOrder(1000)]
public class MinimapController : MonoBehaviour
{
    [System.Serializable]
    public class MapSettings
    {
        [Min(0.1f)]
        public float raio = 12f;

        [Min(0.1f)]
        public float alcanceVertical = 10f;

        [Min(0.01f)]
        public float distanciaTransparencia = 3f;

        [Min(0.1f)]
        public float tamanhoOrtografico = 12f;

        [Min(0.1f)]
        public float distanciaCamera = 30f;

        public float deslocamentoVertical = 2f;
        public float deslocamentoFrontal = 2f;

        [Min(1f)]
        public float tamanhoUI = 300f;
    }


    [Header("Referencias")]

    [SerializeField] private Transform jogador;
    [SerializeField] private Camera cameraPrincipal;
    [SerializeField] private Camera cameraMinimapa;
    [SerializeField] private Transform mundoMinimapa;
    [SerializeField] private RectTransform interfaceMinimapa;


    [Header("Rotacao da Camera")]

    [SerializeField] private bool acompanharRotacaoHorizontal = true;

    [SerializeField] private float inclinacaoBase = 35f;

    [SerializeField] private bool acompanharRotacaoVertical = true;

    [Range(0f, 2f)]
    [SerializeField] private float influenciaVertical = 0.5f;

    [SerializeField] private float inclinacaoMinima = 10f;
    [SerializeField] private float inclinacaoMaxima = 65f;


    [Header("Suavizacao")]

    [Min(0.01f)]
    [SerializeField] private float velocidadeRotacao = 12f;

    [Min(0.01f)]
    [SerializeField] private float velocidadeTransicao = 8f;


    [Header("Modo Compacto")]

    [SerializeField]
    private MapSettings compacto =
        new MapSettings
        {
            raio = 12f,
            alcanceVertical = 10f,
            distanciaTransparencia = 3f,
            tamanhoOrtografico = 12f,
            distanciaCamera = 30f,
            deslocamentoVertical = 2f,
            deslocamentoFrontal = 2f,
            tamanhoUI = 300f
        };


    [Header("Modo Expandido")]

    [SerializeField]
    private MapSettings expandido =
        new MapSettings
        {
            raio = 24f,
            alcanceVertical = 15f,
            distanciaTransparencia = 5f,
            tamanhoOrtografico = 22f,
            distanciaCamera = 45f,
            deslocamentoVertical = 4f,
            deslocamentoFrontal = 4f,
            tamanhoUI = 480f
        };


    [Header("Pulso Especial")]

    [SerializeField] private bool permitirPulso = true;

    [SerializeField]
    private MapSettings pulso =
        new MapSettings
        {
            raio = 40f,
            alcanceVertical = 25f,
            distanciaTransparencia = 6f,
            tamanhoOrtografico = 38f,
            distanciaCamera = 60f,
            deslocamentoVertical = 5f,
            deslocamentoFrontal = 8f,
            tamanhoUI = 550f
        };

    [Min(0.01f)]
    [SerializeField] private float tempoExpansao = 0.6f;

    [Min(0f)]
    [SerializeField] private float tempoPermanencia = 3f;

    [Min(0.01f)]
    [SerializeField] private float tempoRetorno = 1f;

    [Min(0f)]
    [SerializeField] private float cooldownPulso = 10f;


    [Header("Desempenho")]
    [SerializeField] private bool limitarFPSMinimapa = true;
    [Min(1)][SerializeField] private int fpsCompacto = 30;
    [Min(1)][SerializeField] private int fpsExpandido = 30;
    [Min(1)][SerializeField] private int fpsPulso = 60;

#if UNITY_EDITOR
    [Header("Desenvolvimento - Editor Only")]
    [SerializeField] private bool sincronizarMateriaisEmRuntime = true;
    [Min(0.05f)]
    [SerializeField] private float intervaloSincronizacaoMateriais = 0.2f;
    private double proximaSincronizacaoMateriais = -1;
#endif

    [Header("Estado")]

    [SerializeField] private bool mapaExpandido = false;
    [SerializeField] private bool mapaVisivel = true;


    [Header("Controles")]

    [SerializeField] private bool utilizarAtalhos = true;

#if ENABLE_INPUT_SYSTEM

    [SerializeField] private Key teclaExpandir = Key.Tab;
    [SerializeField] private Key teclaVisibilidade = Key.M;
    [SerializeField] private Key teclaPulso = Key.Q;

#elif ENABLE_LEGACY_INPUT_MANAGER

    [SerializeField] private KeyCode teclaExpandir = KeyCode.Tab;
    [SerializeField] private KeyCode teclaVisibilidade = KeyCode.M;
    [SerializeField] private KeyCode teclaPulso = KeyCode.Q;

#endif


    // Referencias internas do Shader Graph.

    private const string PropriedadePosicao =
        "_PosicaoPlayer";

    private const string PropriedadeDirecao =
        "_DirecaoCamera";

    private const string PropriedadeRaio =
        "_RaioMapa";

    private const string PropriedadeTransparencia =
        "_DistanciaTranspareciaBordas";

    private const string PropriedadeVertical =
        "_AlcanceVertical";


    private class ShaderBinding
    {
        public Material material;
        public Renderer renderer;
        public int materialIndex;
        public MaterialPropertyBlock blocoOriginal;
        public int posicao, direcao, raio, transparencia, vertical;
        public bool TemPropriedades => posicao != -1 || direcao != -1 ||
            raio != -1 || transparencia != -1 || vertical != -1;
    }

    // Um clone por material original, nao por mesh.
    private readonly Dictionary<Material, Material> materiaisRuntime =
        new Dictionary<Material, Material>();
    private readonly Dictionary<Material, Material> originaisPorClone =
        new Dictionary<Material, Material>();
    private readonly List<Renderer> renderersAlterados = new List<Renderer>();
    private readonly List<ShaderBinding> bindings = new List<ShaderBinding>();
    private readonly List<ShaderBinding> bindingsIndividuais = new List<ShaderBinding>();
    private MaterialPropertyBlock propertyBlock;

    private bool shaderInicializado;
    private Vector3 ultimaPosicao, ultimaDirecao;
    private float ultimoRaio, ultimaAltura, ultimaTransparencia;
    private float ultimoTamanhoUI = -1f;

    private double proximaRenderizacao = -1;
    private double ultimaAtualizacao = -1;
    private int ultimoFPS = -1;
    private bool renderizarNesteFrame;
    private MinimapRenderGate renderGate;

    private static readonly ProfilerMarker marcadorAtualizacao =
        new ProfilerMarker("Minimap.Controller.Update");
    private static readonly ProfilerMarker marcadorMateriais =
        new ProfilerMarker("Minimap.Controller.Materials");

    // Estado atual da camera.

    private float rotacaoHorizontalAtual;
    private float inclinacaoAtual;


    // Configuracoes interpoladas em tempo real.

    private readonly MapSettings normalAtual =
        new MapSettings();

    private readonly MapSettings configuracaoAtual =
        new MapSettings();


    // Estado interno do pulso.

    private float inicioPulso = -1f;
    private float proximoPulso = 0f;

    private float duracaoExpansaoAtual;
    private float duracaoPermanenciaAtual;
    private float duracaoRetornoAtual;

    private bool inicializado;


    // Propriedades publicas para outros sistemas.

    public bool MapaExpandido => mapaExpandido;

    public bool MapaVisivel => mapaVisivel;
    public bool RenderizarNesteFrame => renderizarNesteFrame;
    public int QuantidadeMateriaisRuntime => materiaisRuntime.Count;

    public bool PulsoAtivo =>
        inicioPulso >= 0f &&
        Time.unscaledTime <
        inicioPulso +
        duracaoExpansaoAtual +
        duracaoPermanenciaAtual +
        duracaoRetornoAtual;

    public bool PulsoDisponivel =>
        inicializado &&
        permitirPulso &&
        mapaVisivel &&
        !PulsoAtivo &&
        Time.unscaledTime >= proximoPulso;

    public float TempoRestantePulso =>
        Mathf.Max(0f, proximoPulso - Time.unscaledTime);

    public float RaioAtual =>
        configuracaoAtual.raio;

    public float AlcanceVerticalAtual =>
        configuracaoAtual.alcanceVertical;


    private void Awake()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (jogador == null ||
            cameraPrincipal == null ||
            cameraMinimapa == null ||
            mundoMinimapa == null ||
            cameraMinimapa == cameraPrincipal)
        {
            enabled = false;
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        cameraMinimapa.orthographic = true;

        // Executa depois de MinimapObjective (ordem 1100).
        renderGate = GetComponent<MinimapRenderGate>();
        if (renderGate == null)
            renderGate = gameObject.AddComponent<MinimapRenderGate>();
        renderGate.DefinirController(this);

        RefreshRenderers();
        InicializarValores();

        inicializado = true;

        AtualizarVisibilidade();
        AtualizarCamera(1f);
        AtualizarShader();
        AtualizarInterface();
    }


    private void InicializarValores()
    {
        MapSettings configuracao =
            mapaExpandido ? expandido : compacto;

        rotacaoHorizontalAtual =
            cameraPrincipal.transform.eulerAngles.y;

        inclinacaoAtual = Mathf.Clamp(
            inclinacaoBase,
            inclinacaoMinima,
            inclinacaoMaxima
        );

        InterpolarConfiguracoes(
            normalAtual,
            configuracao,
            configuracao,
            1f
        );

        InterpolarConfiguracoes(
            configuracaoAtual,
            configuracao,
            configuracao,
            1f
        );
    }


    private void Update()
    {
        if (!inicializado || !utilizarAtalhos)
            return;

#if ENABLE_INPUT_SYSTEM

        Keyboard teclado = Keyboard.current;

        if (teclado == null)
            return;

        if (teclaExpandir != Key.None &&
            teclado[teclaExpandir].wasPressedThisFrame)
        {
            AlternarTamanho();
        }

        if (teclaVisibilidade != Key.None &&
            teclado[teclaVisibilidade].wasPressedThisFrame)
        {
            AlternarVisibilidade();
        }

        if (teclaPulso != Key.None &&
            teclado[teclaPulso].wasPressedThisFrame)
        {
            AtivarPulso();
        }

#elif ENABLE_LEGACY_INPUT_MANAGER

        if (Input.GetKeyDown(teclaExpandir))
            AlternarTamanho();

        if (Input.GetKeyDown(teclaVisibilidade))
            AlternarVisibilidade();

        if (Input.GetKeyDown(teclaPulso))
            AtivarPulso();

#endif
    }


    private void OnEnable()
    {
        SolicitarRenderizacao();
    }

    private void LateUpdate()
    {
        renderizarNesteFrame = false;
        if (!inicializado)
            return;

        AtualizarVisibilidade();
        if (!mapaVisivel || jogador == null || cameraPrincipal == null ||
            cameraMinimapa == null || !cameraMinimapa.gameObject.activeInHierarchy)
        {
            SolicitarRenderizacao();
            return;
        }

        double agora = Time.unscaledTimeAsDouble;
        int fps = !limitarFPSMinimapa ? 0 : Mathf.Max(1,
            PulsoAtivo ? fpsPulso : (mapaExpandido ? fpsExpandido : fpsCompacto));

        if (fps != ultimoFPS)
        {
            ultimoFPS = fps;
            proximaRenderizacao = -1;
        }

        if (fps > 0 && agora < proximaRenderizacao)
            return;

        if (fps > 0)
        {
            double intervalo = 1.0 / fps;
            // Mantem a cadencia media, sem tentar renderizar frames atrasados.
            proximaRenderizacao = proximaRenderizacao < 0
                ? agora + intervalo
                : System.Math.Max(proximaRenderizacao + intervalo, agora + 0.000001);
        }
        else
        {
            proximaRenderizacao = -1;
        }

        float delta = ultimaAtualizacao < 0
            ? Time.unscaledDeltaTime
            : (float)(agora - ultimaAtualizacao);
        ultimaAtualizacao = agora;
        renderizarNesteFrame = true;

        using (marcadorAtualizacao.Auto())
        {
            AtualizarConfiguracoes(1f - Mathf.Exp(-velocidadeTransicao * delta));
            AtualizarCamera(1f - Mathf.Exp(-velocidadeRotacao * delta));
            AtualizarShader();
            AtualizarInterface();
        }
    }

    private void SolicitarRenderizacao()
    {
        proximaRenderizacao = -1;
        ultimaAtualizacao = -1;
    }

    // Chamado apos o LateUpdate dos objetivos, antes da renderizacao normal.
    public void AplicarLimiteRenderizacao()
    {
        if (cameraMinimapa == null || cameraMinimapa == cameraPrincipal)
            return;
        bool deveRenderizar = isActiveAndEnabled && inicializado &&
            mapaVisivel && renderizarNesteFrame;
        if (cameraMinimapa.enabled != deveRenderizar)
            cameraMinimapa.enabled = deveRenderizar;
    }

    private void OnDisable()
    {
        renderizarNesteFrame = false;
        if (cameraMinimapa != null && cameraMinimapa != cameraPrincipal)
            cameraMinimapa.enabled = false;
        if (interfaceMinimapa != null && interfaceMinimapa.gameObject != gameObject)
            interfaceMinimapa.gameObject.SetActive(false);
        SolicitarRenderizacao();
    }

    private void OnDestroy()
    {
        LiberarMateriais();
    }

    // ==================================================
    // CONFIGURACOES E TRANSICOES
    // ==================================================

    private void AtualizarConfiguracoes(float t)
    {
        MapSettings configuracaoNormal =
            mapaExpandido ? expandido : compacto;

        // Suaviza a transicao entre compacto e expandido.

        InterpolarConfiguracoes(
            normalAtual,
            normalAtual,
            configuracaoNormal,
            t
        );

        // Aplica o pulso sobre o estado normal.

        float fatorPulso = CalcularFatorPulso();

        InterpolarConfiguracoes(
            configuracaoAtual,
            normalAtual,
            pulso,
            fatorPulso
        );
    }


    private void InterpolarConfiguracoes(
        MapSettings destino,
        MapSettings origem,
        MapSettings alvo,
        float t)
    {
        destino.raio = Mathf.Lerp(
            origem.raio,
            alvo.raio,
            t
        );

        destino.alcanceVertical = Mathf.Lerp(
            origem.alcanceVertical,
            alvo.alcanceVertical,
            t
        );

        destino.distanciaTransparencia = Mathf.Lerp(
            origem.distanciaTransparencia,
            alvo.distanciaTransparencia,
            t
        );

        destino.tamanhoOrtografico = Mathf.Lerp(
            origem.tamanhoOrtografico,
            alvo.tamanhoOrtografico,
            t
        );

        destino.distanciaCamera = Mathf.Lerp(
            origem.distanciaCamera,
            alvo.distanciaCamera,
            t
        );

        destino.deslocamentoVertical = Mathf.Lerp(
            origem.deslocamentoVertical,
            alvo.deslocamentoVertical,
            t
        );

        destino.deslocamentoFrontal = Mathf.Lerp(
            origem.deslocamentoFrontal,
            alvo.deslocamentoFrontal,
            t
        );

        destino.tamanhoUI = Mathf.Lerp(
            origem.tamanhoUI,
            alvo.tamanhoUI,
            t
        );
    }


    // ==================================================
    // PULSO ESPECIAL
    // ==================================================

    public void AtivarPulso()
    {
        if (!PulsoDisponivel)
            return;

        duracaoExpansaoAtual =
            Mathf.Max(0.01f, tempoExpansao);

        duracaoPermanenciaAtual =
            Mathf.Max(0f, tempoPermanencia);

        duracaoRetornoAtual =
            Mathf.Max(0.01f, tempoRetorno);

        inicioPulso = Time.unscaledTime;

        proximoPulso =
            inicioPulso +
            duracaoExpansaoAtual +
            duracaoPermanenciaAtual +
            duracaoRetornoAtual +
            Mathf.Max(0f, cooldownPulso);
        proximaRenderizacao = -1;
    }


    private float CalcularFatorPulso()
    {
        if (inicioPulso < 0f)
            return 0f;

        float tempo =
            Time.unscaledTime - inicioPulso;


        // Fase 1: Expansao.

        if (tempo < duracaoExpansaoAtual)
        {
            return Mathf.SmoothStep(
                0f,
                1f,
                tempo / duracaoExpansaoAtual
            );
        }

        tempo -= duracaoExpansaoAtual;


        // Fase 2: Permanencia.

        if (tempo < duracaoPermanenciaAtual)
            return 1f;

        tempo -= duracaoPermanenciaAtual;


        // Fase 3: Retorno.

        if (tempo < duracaoRetornoAtual)
        {
            return Mathf.SmoothStep(
                1f,
                0f,
                tempo / duracaoRetornoAtual
            );
        }


        // Finalizacao do pulso.

        inicioPulso = -1f;

        return 0f;
    }


    public void CancelarPulso()
    {
        inicioPulso = -1f;
        proximaRenderizacao = -1;
    }


    // ==================================================
    // CAMERA
    // ==================================================

    private void AtualizarCamera(float t)
    {
        Vector3 direcao =
            cameraPrincipal.transform.forward;

        Vector3 direcaoHorizontal =
            new Vector3(
                direcao.x,
                0f,
                direcao.z
            );

        float rotacaoAlvo =
            rotacaoHorizontalAtual;

        if (acompanharRotacaoHorizontal &&
            direcaoHorizontal.sqrMagnitude > 0.0001f)
        {
            rotacaoAlvo = Mathf.Atan2(
                direcaoHorizontal.x,
                direcaoHorizontal.z
            ) * Mathf.Rad2Deg;
        }

        float inclinacaoAlvo =
            inclinacaoBase;

        if (acompanharRotacaoVertical)
        {
            float inclinacaoPrincipal =
                Mathf.Atan2(
                    -direcao.y,
                    direcaoHorizontal.magnitude
                ) * Mathf.Rad2Deg;

            inclinacaoAlvo +=
                inclinacaoPrincipal *
                influenciaVertical;
        }

        inclinacaoAlvo = Mathf.Clamp(
            inclinacaoAlvo,
            inclinacaoMinima,
            inclinacaoMaxima
        );

        rotacaoHorizontalAtual = Mathf.LerpAngle(
            rotacaoHorizontalAtual,
            rotacaoAlvo,
            t
        );

        inclinacaoAtual = Mathf.Lerp(
            inclinacaoAtual,
            inclinacaoAlvo,
            t
        );

        Quaternion rotacao = Quaternion.Euler(
            inclinacaoAtual,
            rotacaoHorizontalAtual,
            0f
        );

        Vector3 frenteHorizontal =
            Quaternion.Euler(
                0f,
                rotacaoHorizontalAtual,
                0f
            ) * Vector3.forward;

        Vector3 centro =
            jogador.position
            + Vector3.up *
            configuracaoAtual.deslocamentoVertical
            + frenteHorizontal *
            configuracaoAtual.deslocamentoFrontal;

        Vector3 posicaoCamera =
            centro -
            rotacao * Vector3.forward *
            configuracaoAtual.distanciaCamera;

        cameraMinimapa.transform.SetPositionAndRotation(
            posicaoCamera,
            rotacao
        );

        cameraMinimapa.orthographicSize =
            Mathf.Max(
                0.1f,
                configuracaoAtual.tamanhoOrtografico
            );
    }


    // ==================================================
    // SHADER GRAPH
    // ==================================================

    private void AtualizarShader()
    {
        using (marcadorMateriais.Auto())
        {
#if UNITY_EDITOR
            SincronizarMateriaisEditor();
#endif
            Vector3 posicao = jogador.position;
            Vector3 direcao = Quaternion.Euler(0f, rotacaoHorizontalAtual, 0f) * Vector3.forward;
            float raio = Mathf.Max(0.1f, configuracaoAtual.raio);
            float altura = Mathf.Max(0.1f, configuracaoAtual.alcanceVertical);
            float transparencia = Mathf.Clamp(configuracaoAtual.distanciaTransparencia,
                0.001f, raio - 0.001f);

            bool mudouPosicao = !shaderInicializado || !posicao.Equals(ultimaPosicao);
            bool mudouDirecao = !shaderInicializado || !direcao.Equals(ultimaDirecao);
            bool mudouRaio = !shaderInicializado || raio != ultimoRaio;
            bool mudouAltura = !shaderInicializado || altura != ultimaAltura;
            bool mudouTransparencia = !shaderInicializado || transparencia != ultimaTransparencia;

            if (mudouPosicao || mudouDirecao || mudouRaio || mudouAltura || mudouTransparencia)
            {
                foreach (ShaderBinding b in bindings)
                {
                    Material m = b.material;
                    if (m == null) continue;
                    if (mudouPosicao && b.posicao != -1) m.SetVector(b.posicao, posicao);
                    if (mudouDirecao && b.direcao != -1) m.SetVector(b.direcao, direcao);
                    if (mudouRaio && b.raio != -1) m.SetFloat(b.raio, raio);
                    if (mudouAltura && b.vertical != -1) m.SetFloat(b.vertical, altura);
                    if (mudouTransparencia && b.transparencia != -1) m.SetFloat(b.transparencia, transparencia);
                }
            }

            // Compatibilidade: objetos que JA tinham PropertyBlock continuam
            // usando o caminho antigo, preservando suas propriedades individuais.
            foreach (ShaderBinding b in bindingsIndividuais)
            {
                Renderer r = b.renderer;
                if (r == null || !r.gameObject.activeInHierarchy) continue;
                r.GetPropertyBlock(propertyBlock, b.materialIndex);
                if (propertyBlock.isEmpty) r.GetPropertyBlock(propertyBlock);
                if (b.posicao != -1) propertyBlock.SetVector(b.posicao, posicao);
                if (b.direcao != -1) propertyBlock.SetVector(b.direcao, direcao);
                if (b.raio != -1) propertyBlock.SetFloat(b.raio, raio);
                if (b.vertical != -1) propertyBlock.SetFloat(b.vertical, altura);
                if (b.transparencia != -1) propertyBlock.SetFloat(b.transparencia, transparencia);
                r.SetPropertyBlock(propertyBlock, b.materialIndex);
            }

            ultimaPosicao = posicao;
            ultimaDirecao = direcao;
            ultimoRaio = raio;
            ultimaAltura = altura;
            ultimaTransparencia = transparencia;
            shaderInicializado = true;
        }
    }

#if UNITY_EDITOR
    // Permite editar os assets originais durante o Play Mode, sem alterar
    // a estrategia de materiais compartilhados usada na build.
    private void SincronizarMateriaisEditor()
    {
        if (!sincronizarMateriaisEmRuntime)
        {
            proximaSincronizacaoMateriais = -1;
            return;
        }

        double agora = Time.unscaledTimeAsDouble;
        if (agora < proximaSincronizacaoMateriais)
            return;

        proximaSincronizacaoMateriais = agora +
            Mathf.Max(0.05f, intervaloSincronizacaoMateriais);

        bool copiou = false;
        foreach (KeyValuePair<Material, Material> par in materiaisRuntime)
        {
            Material original = par.Key;
            Material runtime = par.Value;
            if (original == null || runtime == null || original.shader != runtime.shader)
                continue;

            runtime.CopyPropertiesFromMaterial(original);
            copiou = true;
        }

        // A copia inclui as propriedades dinamicas. Forca a reaplicacao
        // das cinco propriedades do controlador no mesmo frame, antes de renderizar.
        if (copiou)
            shaderInicializado = false;
    }
#endif

    private ShaderBinding CriarBinding(Material material)
    {
        return new ShaderBinding
        {
            material = material,
            posicao = EncontrarPropriedade(material, PropriedadePosicao, "PosicaoPlayer", "_PlayerPosition"),
            direcao = EncontrarPropriedade(material, PropriedadeDirecao, "DirecaoCamera", "_CameraForward"),
            raio = EncontrarPropriedade(material, PropriedadeRaio, "RaioMapa", "_MapRadius"),
            transparencia = EncontrarPropriedade(material, PropriedadeTransparencia,
                "DistanciaTranspareciaBordas", "_DistanciaTransparenciaBordas",
                "DistanciaTransparenciaBordas", "_FadeDistance"),
            vertical = EncontrarPropriedade(material, PropriedadeVertical, "AlcanceVertical")
        };
    }

    public void RefreshRenderers()
    {
        if (!Application.isPlaying) return;
        LiberarMateriais();
        if (mundoMinimapa == null) return;
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        // Cache inclusive materiais que nao pertencem ao shader do mapa.
        var cache = new Dictionary<Material, ShaderBinding>();
        foreach (Renderer r in mundoMinimapa.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materiais = r.sharedMaterials;
            bool alterou = false;
            bool temBloco = r.HasPropertyBlock();
            for (int i = 0; i < materiais.Length; i++)
            {
                Material original = materiais[i];
                if (original == null) continue;

                if (!cache.TryGetValue(original, out ShaderBinding modelo))
                {
                    modelo = CriarBinding(original);
                    cache.Add(original, modelo);
                }
                if (!modelo.TemPropriedades) continue;

                if (temBloco)
                {
                    ShaderBinding individual = CriarBinding(original);
                    individual.renderer = r;
                    individual.materialIndex = i;
                    individual.blocoOriginal = new MaterialPropertyBlock();
                    r.GetPropertyBlock(individual.blocoOriginal, i);
                    bindingsIndividuais.Add(individual);
                    continue;
                }

                if (!materiaisRuntime.TryGetValue(original, out Material runtime))
                {
                    runtime = new Material(original);
                    runtime.name = original.name + " (Minimap Runtime)";
                    runtime.hideFlags = HideFlags.DontSave;
                    materiaisRuntime.Add(original, runtime);
                    originaisPorClone.Add(runtime, original);
                    bindings.Add(CriarBinding(runtime));
                }
                materiais[i] = runtime;
                alterou = true;
            }
            if (alterou)
            {
                r.sharedMaterials = materiais;
                renderersAlterados.Add(r);
            }
        }

        shaderInicializado = false;
        proximaRenderizacao = -1;
#if UNITY_EDITOR
        proximaSincronizacaoMateriais = -1;
#endif
    }

    private void LiberarMateriais()
    {
        // Restaura somente slots que ainda usam um clone deste controller.
        foreach (Renderer r in renderersAlterados)
        {
            if (r == null) continue;
            Material[] materiais = r.sharedMaterials;
            bool alterou = false;
            for (int i = 0; i < materiais.Length; i++)
            {
                Material m = materiais[i];
                if (m != null && originaisPorClone.TryGetValue(m, out Material original))
                {
                    materiais[i] = original;
                    alterou = true;
                }
            }
            if (alterou) r.sharedMaterials = materiais;
        }
        foreach (ShaderBinding b in bindingsIndividuais)
        {
            if (b.renderer == null || b.materialIndex >= b.renderer.sharedMaterials.Length) continue;
            b.renderer.SetPropertyBlock(b.blocoOriginal.isEmpty ? null : b.blocoOriginal, b.materialIndex);
        }
        foreach (Material m in materiaisRuntime.Values)
            if (m != null) Destroy(m);

        bindings.Clear();
        bindingsIndividuais.Clear();
        materiaisRuntime.Clear();
        originaisPorClone.Clear();
        renderersAlterados.Clear();
        shaderInicializado = false;
    }

    private int EncontrarPropriedade(
        Material material,
        params string[] nomes)
    {
        foreach (string nome in nomes)
        {
            if (material.HasProperty(nome))
                return Shader.PropertyToID(nome);
        }

        return -1;
    }


    // ==================================================
    // INTERFACE E VISIBILIDADE
    // ==================================================

    private void AtualizarInterface()
    {
        if (interfaceMinimapa == null)
            return;

        float tamanho =
            Mathf.Max(
                1f,
                configuracaoAtual.tamanhoUI
            );

        if (Mathf.Abs(tamanho - ultimoTamanhoUI) < 0.01f) return;
        ultimoTamanhoUI = tamanho;

        interfaceMinimapa.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            tamanho
        );

        interfaceMinimapa.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            tamanho
        );
    }


    private void AtualizarVisibilidade()
    {
        if (cameraMinimapa != null && cameraMinimapa != cameraPrincipal &&
            cameraMinimapa.enabled != mapaVisivel)
        {
            cameraMinimapa.enabled = mapaVisivel;
        }

        if (interfaceMinimapa != null &&
            interfaceMinimapa.gameObject.activeSelf != mapaVisivel)
        {
            interfaceMinimapa.gameObject.SetActive(
                mapaVisivel
            );
        }
    }


    // ==================================================
    // METODOS PUBLICOS
    // ==================================================

    public void AlternarTamanho()
    {
        mapaExpandido = !mapaExpandido;
        proximaRenderizacao = -1;
    }


    public void Expandir()
    {
        mapaExpandido = true;
        proximaRenderizacao = -1;
    }


    public void Recolher()
    {
        mapaExpandido = false;
        proximaRenderizacao = -1;
    }


    public void AlternarVisibilidade()
    {
        DefinirVisibilidade(!mapaVisivel);
    }


    public void DefinirVisibilidade(bool visivel)
    {
        if (mapaVisivel != visivel) SolicitarRenderizacao();
        mapaVisivel = visivel;
        AtualizarVisibilidade();
    }


    public void DefinirCameraPrincipal(Camera novaCamera)
    {
        if (novaCamera != null && novaCamera != cameraMinimapa)
        {
            cameraPrincipal = novaCamera;
            proximaRenderizacao = -1;
        }
    }


    // Altera somente as duas layers temporais do minimapa.
    // Preserva todas as outras layers ja configuradas na camera.
    public void DefinirLinhaTemporal(bool noPassado)
    {
        if (cameraMinimapa == null || cameraMinimapa == cameraPrincipal)
            return;

        int layerPassado = LayerMask.NameToLayer("MinimapPast");
        int layerFuturo = LayerMask.NameToLayer("MinimapFuture");

        // Evita alterar a mascara se a configuracao estiver incompleta.
        if (layerPassado < 0 || layerFuturo < 0)
            return;

        int mascaraPassado = 1 << layerPassado;
        int mascaraFuturo = 1 << layerFuturo;

        int mascara = cameraMinimapa.cullingMask;
        mascara &= ~(mascaraPassado | mascaraFuturo);
        mascara |= noPassado ? mascaraPassado : mascaraFuturo;

        cameraMinimapa.cullingMask = mascara;

        // Atualiza a imagem no proximo LateUpdate, sem esperar o limite de FPS.
        // Nao habilita a camera se o minimapa estiver oculto.
        proximaRenderizacao = -1;
    }

    // Compatibilidade com os metodos anteriores.

    public void ToggleExpanded()
    {
        AlternarTamanho();
    }


    public void Expand()
    {
        Expandir();
    }


    public void Compact()
    {
        Recolher();
    }


    // ==================================================
    // RESTAURAR CONFIGURACOES
    // ==================================================

    [ContextMenu("Restaurar configuracoes dos modos")]
    private void RestaurarConfiguracoes()
    {
        compacto = new MapSettings
        {
            raio = 12f,
            alcanceVertical = 10f,
            distanciaTransparencia = 3f,
            tamanhoOrtografico = 12f,
            distanciaCamera = 30f,
            deslocamentoVertical = 2f,
            deslocamentoFrontal = 2f,
            tamanhoUI = 300f
        };

        expandido = new MapSettings
        {
            raio = 24f,
            alcanceVertical = 15f,
            distanciaTransparencia = 5f,
            tamanhoOrtografico = 22f,
            distanciaCamera = 45f,
            deslocamentoVertical = 4f,
            deslocamentoFrontal = 4f,
            tamanhoUI = 480f
        };

        pulso = new MapSettings
        {
            raio = 40f,
            alcanceVertical = 25f,
            distanciaTransparencia = 6f,
            tamanhoOrtografico = 38f,
            distanciaCamera = 60f,
            deslocamentoVertical = 5f,
            deslocamentoFrontal = 8f,
            tamanhoUI = 550f
        };

        mapaExpandido = false;
    }
}