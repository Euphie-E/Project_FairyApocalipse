using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropshadowController : MonoBehaviour
{
    static public DropshadowController Instance;
    DecalProjector decal;

    void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }

        decal = GetComponent<DecalProjector>();
        ActiveDropshadow();
    }

    public void ActiveDropshadow()
    {
        decal.enabled = true;
    }

    public void DisableDropshadow()
    {
        decal.enabled = false;
    }
}
