using UnityEngine;

public class TonguePreTip : MonoBehaviour
{
    [SerializeField] TongueController tongue;

    void OnTriggerEnter(Collider other)
    {
        tongue.OnCollision();
    }
}
