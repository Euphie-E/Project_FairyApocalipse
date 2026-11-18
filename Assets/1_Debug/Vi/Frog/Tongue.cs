using UnityEngine;

public class Tongue : MonoBehaviour
{
    [SerializeField] Tongue tongueNext;
    [SerializeField] bool anchor;
    [Tooltip("Apenas o ultimo antes da ancora vai usar isso")] 
    [SerializeField] Transform mouthPoint;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
