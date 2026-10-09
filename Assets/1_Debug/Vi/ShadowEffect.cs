using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowEffect : MonoBehaviour
{
    private DecalProjector decalProjector;
    bool isFalling = false;
    float startHeight;
    float differenceHeight = 25;
    float floorHeight;

    void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
    }

    void Onable()
    {
        isFalling = false;
    }

    void Update()
    {
        HandleOpacity();
    }

    private void HandleOpacity()
    {
        if (isFalling)
        {
            float opacity = 1 - ((transform.position.y - floorHeight) / differenceHeight);
            if (opacity <= 1) decalProjector.fadeFactor = opacity;
            else decalProjector.fadeFactor = 1;
        }
    }

    public void StartDrop(float spawnHeight)
    {
        isFalling = true;
        differenceHeight = spawnHeight;
        startHeight = transform.position.y;
        floorHeight = startHeight - differenceHeight;
    }

}
