
using UnityEngine;

public class MushroomTransformSpringVisual : MonoBehaviour
{
    [Header("Modelo do Cogumelo")]
    public Transform mushroomVisual;

    [Header("Deformacao")]
    [Range(0f, 0.8f)]
    public float squashY = 0.35f;

    [Range(0f, 0.8f)]
    public float stretchXZ = 0.2f;

    [Header("Efeito de Mola")]
    public float squashTime = 0.08f;
    public float springDuration = 0.55f;
    public float springFrequency = 3f;
    public float damping = 6f;

    private Vector3 originalScale;
    private float timer;
    private bool playing;

    private void Awake()
    {
        if (mushroomVisual != null)
            originalScale = mushroomVisual.localScale;
    }

    public void Play()
    {
        if (mushroomVisual == null)
            return;

        timer = 0f;
        playing = true;
    }

    private void LateUpdate()
    {
        if (!playing || mushroomVisual == null)
            return;

        timer += Time.deltaTime;

        float amount;

        if (timer < squashTime)
        {
            float t = Mathf.Clamp01(
                timer / Mathf.Max(0.001f, squashTime)
            );

            amount = Mathf.SmoothStep(0f, 1f, t);
        }
        else
        {
            float t = timer - squashTime;

            amount =
                Mathf.Exp(-damping * t) *
                Mathf.Cos(
                    t * springFrequency * Mathf.PI * 2f
                );
        }

        float scaleY = 1f - squashY * amount;
        float scaleXZ = 1f + stretchXZ * amount;

        mushroomVisual.localScale = Vector3.Scale(
            originalScale,
            new Vector3(scaleXZ, scaleY, scaleXZ)
        );

        if (timer >= squashTime + springDuration)
        {
            mushroomVisual.localScale = originalScale;
            playing = false;
        }
    }

    private void OnDisable()
    {
        if (playing && mushroomVisual != null)
            mushroomVisual.localScale = originalScale;

        playing = false;
    }
}
