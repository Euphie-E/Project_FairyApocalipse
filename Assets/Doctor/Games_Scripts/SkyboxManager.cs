using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public static SkyboxManager Instance;

    [Header("Directional Light")]
    public Transform directionalLight;

    [Header("Skyboxes")]
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Artistic Rotations")]
    public float morningRotation = 20f;
    public float afternoonRotation = 70f;
    public float middayRotation = 120f;
    public float sunsetRotation = 170f;
    public float nightRotation = 220f;


     public bool isNight = false;
    private void Start()
    {
        SetDayStage(0);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SetDayStage(int stage)
    {
        RenderSettings.skybox = daySkybox;
        

        float rotation = morningRotation;

        switch (stage)
        {
            case 0:
                rotation = morningRotation;
                break;

            case 1:
                rotation = afternoonRotation;
                break;

            case 2:
                rotation = middayRotation;
                break;

            case 3:
                rotation = sunsetRotation;
                break;
        }

        directionalLight.rotation =
            Quaternion.Euler(rotation, 0, 0);

        DynamicGI.UpdateEnvironment();

        Debug.Log($"Sky Stage Changed: {stage}");
    }

    public void SetNight()
    {
        isNight = true;
        RenderSettings.skybox = nightSkybox;
       

        directionalLight.rotation =
            Quaternion.Euler(nightRotation, 0, 0);

        DynamicGI.UpdateEnvironment();

        Debug.Log("Night Started!");
    }
}