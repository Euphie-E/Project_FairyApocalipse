using UnityEngine;

public class RainController : MonoBehaviour
{
    [Header("Tempestade")]
    [SerializeField] private StormVFX tempestade;
    void Start()
    {
        
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha5))
            tempestade.AlternarTempestade();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            tempestade?.ChuvaFraca();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            tempestade?.ChuvaForte();

        if (Input.GetKeyDown(KeyCode.Alpha8))
            tempestade?.Temporal();
    }


}
