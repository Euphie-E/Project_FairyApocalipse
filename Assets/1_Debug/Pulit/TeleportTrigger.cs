using System.Collections;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Teleport")]
    public Transform teleportPoint;

    public bool nitghChange = false;

    [Header("Fade")]
    public CanvasGroup blackScreen;

    public float fadeDuration = 0.5f;
    public float blackScreenDuration = 2f;

    [Header("Proteção")]
    public float teleportCooldown = 1f;

    private static bool teleporting = false;

    private void Start()
    {
            blackScreen.alpha = 0f;
            blackScreen.blocksRaycasts = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (teleporting)
            return;

        // Procura o PlayerMovement no objeto que entrou
        // ou em algum pai dele
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        StartCoroutine(Teleport(player));
    }

    private IEnumerator Teleport(PlayerMovement player)
    {
        teleporting = true;

        // Fade para preto
        yield return StartCoroutine(Fade(0f, 1f));

        // Teleporta através do próprio PlayerMovement
        player.TeleportTo(teleportPoint);

        if(nitghChange)
            SkyboxManager.Instance.SetNight();

        // Mantém tela preta
        yield return new WaitForSeconds(blackScreenDuration);

        // Fade voltando
        yield return StartCoroutine(Fade(1f, 0f));

        // Tempo de segurança
        yield return new WaitForSeconds(teleportCooldown);

        teleporting = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            blackScreen.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                timer / fadeDuration
            );

            yield return null;
        }

        blackScreen.alpha = endAlpha;
    }
}