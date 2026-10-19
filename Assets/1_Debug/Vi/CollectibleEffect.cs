using UnityEngine;

public class CollectibleEffect : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float verticalAmplitude = 2;
    public float verticalSpeed = 1;
    Vector3 originalPos;
    float timeVariation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timeVariation = Mathf.Abs(Mathf.Sin(Time.time * 1.5f)) * 0.45f + 0.55f;
        transform.Rotate(Vector3.up, rotationSpeed/10 * timeVariation);
        transform.position = originalPos + transform.up * verticalAmplitude/100 * Mathf.Sin(Time.time * verticalSpeed);
    }
}