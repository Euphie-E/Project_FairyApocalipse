
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowEffect : MonoBehaviour
{
    private DecalProjector decalProjector;
    private Transform rock;

    private bool isFalling;
    private float differenceHeight;
    private float floorHeight;

    [SerializeField] private float groundOffset = 0.05f;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
    }

    private void Update()
    {
        if (!isFalling || rock == null) return;

        float distance = rock.position.y - floorHeight;

        float progress = 1f - (distance / differenceHeight);
        progress = Mathf.Clamp01(progress);

        // A sombra fica mais forte conforme a pedra cai
        decalProjector.fadeFactor = Mathf.Lerp(0.5f, 1f, progress);
    }

    public void ShowWarning(Transform fallingRock, float height)
    {
        rock = fallingRock;
        differenceHeight = Mathf.Max(0.01f, height);

        floorHeight = rock.position.y - differenceHeight;

        // Posiciona a sombra no chão
        transform.position = new Vector3(
            rock.position.x,
            floorHeight + groundOffset,
            rock.position.z
        );

        isFalling = false;
        decalProjector.fadeFactor = 1f;

        gameObject.SetActive(true);
    }

    public void StartDrop()
    {
        isFalling = true;
    }

    public void HideShadow()
    {
        isFalling = false;
        gameObject.SetActive(false);
    }
}
