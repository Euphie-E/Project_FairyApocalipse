using UnityEngine;

public class TearShot : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathController>().Push(transform.position);
        }
    }
}
