using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public static SkyboxManager Instance;

    [Header("Directional Light")]
    public GameObject directionalLight;

    [Header("Skyboxes")]
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Current State")]
    public bool isNight = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateEnvironment();
    }

    public void SetDay()
    {
        isNight = false;
        UpdateEnvironment();
    }

    public void SetNight()
    {
        isNight = true;
        UpdateEnvironment();
    }

    private void UpdateEnvironment()
    {
        if (isNight)
        {
            RenderSettings.skybox = nightSkybox;

            if (directionalLight != null)
                directionalLight.SetActive(false);

            Debug.Log("Night Started!");
        }
        else
        {
            RenderSettings.skybox = daySkybox;

            if (directionalLight != null)
                directionalLight.SetActive(true);

            Debug.Log("Day Started!");
        }

        DynamicGI.UpdateEnvironment();
    }
}
