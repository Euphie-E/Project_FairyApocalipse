using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Movimento")]
    public float floatSpeed = 2f;   // velocidade
    public float floatHeight = 0.2f; // altura do sobe/desce

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.localPosition = new Vector3(
            startPos.x,
            newY,
            startPos.z
        );
    }
}