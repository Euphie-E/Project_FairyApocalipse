using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public static SkyboxManager Instance;

    [Header("Directional Light")]
    public Transform directionalLight;

    [Header("Skyboxes")]
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Time")]
    [Range(0f, 24f)]
    public float timeOfDay = 8f;

    [Tooltip("Quantos minutos reais duram 24 horas no jogo")]
    public float dayDurationMinutes = 10f;

    [Header("Day / Night")]
    [Range(0f, 24f)]
    public float sunriseHour = 6f;

    [Range(0f, 24f)]
    public float sunsetHour = 18f;

    public bool isNight;

    private bool previousNightState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateEnvironment(true);
    }

    private void Update()
    {
        UpdateTime();
        UpdateSun();
        UpdateEnvironment();
    }

    private void UpdateTime()
    {
        // Quantas horas do jogo passam por segundo real
        float gameHoursPerSecond = 24f / (dayDurationMinutes * 60f);

        timeOfDay += gameHoursPerSecond * Time.deltaTime;

        // Volta para 0 depois das 24h
        if (timeOfDay >= 24f)
        {
            timeOfDay -= 24f;
        }
    }

    private void UpdateSun()
    {
        // Converte o horário de 0-24 para uma rotação de 0-360
        float sunRotation = (timeOfDay / 24f) * 360f - 90f;

        directionalLight.rotation =
            Quaternion.Euler(sunRotation, 0f, 0f);
    }

    private void UpdateEnvironment(bool forceUpdate = false)
    {
        isNight =
            timeOfDay < sunriseHour ||
            timeOfDay >= sunsetHour;

        // Só troca o skybox quando realmente muda dia/noite
        if (forceUpdate || isNight != previousNightState)
        {
            if (isNight)
            {
                RenderSettings.skybox = nightSkybox;
                Debug.Log("Night Started!");
            }
            else
            {
                RenderSettings.skybox = daySkybox;
                Debug.Log("Day Started!");
            }

            DynamicGI.UpdateEnvironment();

            previousNightState = isNight;
        }
    }
}