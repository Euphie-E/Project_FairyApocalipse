using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Teleport")]
    public Transform teleportPoint;

    public bool nitghChange = false;

    [Header("Fade")]
    public CanvasGroup blackScreen;
    public Image blackImage;

    public float fadeDuration = 0.5f;
    public float blackScreenDuration = 2f;

    [Header("Proteção")]
    public float teleportCooldown = 1f;

    [Header("Música do Boss")]
    public bool playBossMusic = false;

    private static bool teleporting = false;
    [Header("Rotação do Player")]
    public bool invertPlayerDirection = false;

    [Header("Rotação da Câmera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private bool invertCameraDirection = false;

    private void Start()
    {
        // Mantém a tela invisível ao iniciar
        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;

        // Deixa a imagem preta opaca internamente,
        // mas o CanvasGroup continua escondendo ela.
        if (blackImage != null)
        {
            Color color = blackImage.color;
            color.a = 1f;
            blackImage.color = color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TeleportTrigger detectou: " + other.name);

        if (teleporting)
        {
            Debug.LogWarning("Teleporte já está em andamento!");
            return;
        }

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        Debug.Log("Iniciando transição do teleporte!");

        StartCoroutine(Teleport(player));
    }

    private IEnumerator Teleport(PlayerMovement player)
    {
        teleporting = true;

        // Fade para preto
        yield return StartCoroutine(Fade(0f, 1f));

        player.TeleportTo(teleportPoint);

        // Aguarda a câmera atualizar
        yield return null;

        // Corrige a direção do Player
        player.FaceCameraDirection(invertPlayerDirection);

        // Inverte a posição da câmera ao redor do Player
        if (invertCameraDirection && cinemachineCamera != null)
        {
            CinemachineOrbitalFollow orbital =
                cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

            if (orbital != null)
            {
                orbital.HorizontalAxis.Value += 180f;

                // Atualiza a câmera sem usar o estado anterior
                cinemachineCamera.PreviousStateIsValid = false;
            }
            else
            {
                Debug.LogWarning(
                    "A Cinemachine Camera não possui Orbital Follow!"
                );
            }
        }

        PlayerDeathController deathController =
            player.GetComponent<PlayerDeathController>();

        if (deathController != null)
            deathController.ResetFallAfterTeleport();

        // Altera para noite, se configurado
        if (nitghChange)
            SkyboxManager.Instance.SetNight();

        // Inicia a música do Boss, se ativada
        if (playBossMusic)
        {
            GCamarada.AudioManager.I?.StopMusic();
            GCamarada.AudioManager.I?.PlayClip("boss", true);
           
        }

        // Mantém a tela preta
        yield return new WaitForSeconds(blackScreenDuration);

        // Fade voltando
        yield return StartCoroutine(Fade(1f, 0f));

        // Tempo de segurança
        yield return new WaitForSeconds(teleportCooldown);

        teleporting = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (blackScreen == null)
        {
            Debug.LogError("CanvasGroup não configurado!");
            yield break;
        }

        blackScreen.gameObject.SetActive(true);

        float timer = 0f;

        blackScreen.alpha = startAlpha;

        Debug.Log($"Fade iniciado: {startAlpha} -> {endAlpha}");

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / Mathf.Max(0.01f, fadeDuration));

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                t
            );

            yield return null;
        }

        blackScreen.alpha = endAlpha;

        Debug.Log("Fade concluído. Alpha: " + blackScreen.alpha);
    }
}