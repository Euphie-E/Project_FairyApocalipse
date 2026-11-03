
using UnityEngine;

public class BossAttackWarning : MonoBehaviour
{
    [Header("Visual")]
    public Transform visual;

    [Header("Tempo ate o impacto")]
    [Min(0.01f)]
    public float warningDuration = 1.5f;

    [Header("Pulsacao")]
    public float startFrequency = 1f;
    public float endFrequency = 6f;

    [Range(0f, 0.5f)]
    public float pulseScale = 0.2f;

    private Vector3 originalScale;
    private float timer;
    private bool showing;

    private void Awake()
    {
        if (visual == null)
            return;

        originalScale = visual.localScale;
        visual.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (visual == null)
            return;

        timer = 0f;
        showing = true;

        visual.localScale = originalScale;
        visual.gameObject.SetActive(true);
    }

    public void Hide()
    {
        showing = false;

        if (visual == null)
            return;

        visual.localScale = originalScale;
        visual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!showing || visual == null)
            return;

        timer += Time.deltaTime;

        float duration = Mathf.Max(0.01f, warningDuration);
        float t = Mathf.Min(timer, duration);

        // Frequencia aumenta progressivamente
        float cycles =
            startFrequency * t +
            0.5f * (endFrequency - startFrequency)
            * t * t / duration;

        if (timer > duration)
            cycles += endFrequency * (timer - duration);

        float pulse = Mathf.Sin(
            cycles * Mathf.PI * 2f
        );

        float scale = 1f + pulse * pulseScale;

        visual.localScale = Vector3.Scale(
            originalScale,
            new Vector3(scale, 1f, scale)
        );
    }

    private void OnDisable()
    {
        Hide();
    }
}
