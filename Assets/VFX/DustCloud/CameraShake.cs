using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake")]
    public float duration = 0.4f;
    public float magnitude = 3f;
    public float frequency = 2f;

    private Coroutine shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin currentNoise;

    private void Start()
    {
        var noises = FindObjectsByType<CinemachineBasicMultiChannelPerlin>(FindObjectsSortMode.None);
        foreach (var n in noises)
            n.AmplitudeGain = 0f;
    }

    [ContextMenu("Testar Shake")]
    public void Shake()
    {
        Shake(magnitude, duration, frequency);
    }

    public void Shake(float mag, float dur, float freq)
    {
        var brain = CinemachineBrain.GetActiveBrain(0);
        if (brain == null)
        {
            Debug.LogError("CameraShake: nenhum CinemachineBrain ativo!", this);
            return;
        }

        var vcam = brain.ActiveVirtualCamera as CinemachineCamera;
        if (vcam == null)
        {
            Debug.LogError("CameraShake: camera ativa nao e uma CinemachineCamera!", this);
            return;
        }

        var noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null || noise.NoiseProfile == null)
        {
            Debug.LogError("CameraShake: Noise ou Noise Profile ausente em " + vcam.name, this);
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            if (currentNoise != null) currentNoise.AmplitudeGain = 0f;
        }

        currentNoise = noise;
        shakeCoroutine = StartCoroutine(ShakeCamera(noise, mag, dur, freq));
    }

    private IEnumerator ShakeCamera(CinemachineBasicMultiChannelPerlin noise, float mag, float dur, float freq)
    {
        noise.FrequencyGain = freq;
        noise.AmplitudeGain = mag;

        yield return new WaitForSecondsRealtime(dur);

        noise.AmplitudeGain = 0f;
        shakeCoroutine = null;
    }
}