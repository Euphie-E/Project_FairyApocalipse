using UnityEngine;

public class Collectible : MonoBehaviour
{
    private GameObject collectible;
    public bool Collected = false;
    [Header("Effect")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float verticalAmplitude = 2;
    [SerializeField] private float verticalSpeed = 1;

    Vector3 originalPos;
    float timeVariation;

    void Start()
    {
        collectible = transform.GetChild(0).gameObject;
        originalPos = transform.position;
    }

    void Update()
    {
        if (!Collected)
        {
            timeVariation = Mathf.Abs(Mathf.Sin(Time.time * 1.5f)) * 0.45f + 0.55f;
            transform.Rotate(Vector3.up, rotationSpeed/10 * timeVariation);
            transform.position = originalPos + transform.up * verticalAmplitude/100 * Mathf.Sin(Time.time * verticalSpeed);
        }
    }

    /// <summary>
    /// Chama o efeito após coletar e passa infos para outros códigos.
    /// Chamar código original no fim do override.
    /// </summary>
    protected virtual void Collect()
    {
        Collected = true;
        CollectEffect();
        CollectInfo();
    }

    protected virtual void CollectEffect()
    {
        collectible.SetActive(false);
    }

    /// <summary>
    /// Passa para outros códigos a informação de que o coletavel foi pego.
    /// </summary>
    protected virtual void CollectInfo()
    {
        
    }
}
