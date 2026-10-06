using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Floating")]
    public float floatHeight = 0.15f;
    public float floatSpeed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Billboard
        transform.forward = Camera.main.transform.forward;

        // Floating
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = startPosition + Vector3.up * yOffset;
    }
}