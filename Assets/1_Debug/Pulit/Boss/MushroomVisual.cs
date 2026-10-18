
using UnityEngine;

public class MushroomSpringVisual : MonoBehaviour
{
    [Header("Osso do Cogumelo")]
    public Transform mushroomBone;

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
        if (mushroomBone != null)
            originalScale = mushroomBone.localScale;
    }

    public void Play()
    {
        if (mushroomBone == null)
            return;

        timer = 0f;
        playing = true;
    }

    private void LateUpdate()
    {
        if (!playing || mushroomBone == null)
            return;

        timer += Time.deltaTime;

        float amount;

        if (timer < squashTime)
        {
            // Achatamento rapido
            float t = Mathf.Clamp01(
                timer / Mathf.Max(0.001f, squashTime)
            );

            amount = Mathf.SmoothStep(0f, 1f, t);
        }
        else
        {
            // Oscilacao amortecida da mola
            float t = timer - squashTime;

            amount =
                Mathf.Exp(-damping * t) *
                Mathf.Cos(t * springFrequency * Mathf.PI * 2f);
        }

        float scaleY = 1f - squashY * amount;
        float scaleXZ = 1f + stretchXZ * amount;

        mushroomBone.localScale = Vector3.Scale(
            originalScale,
            new Vector3(scaleXZ, scaleY, scaleXZ)
        );

        if (timer >= squashTime + springDuration)
        {
            mushroomBone.localScale = originalScale;
            playing = false;
        }
    }

    private void OnDisable()
    {
        if (playing && mushroomBone != null)
            mushroomBone.localScale = originalScale;

        playing = false;
    }
}
