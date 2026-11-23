using UnityEngine;

public class Drown : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        FindAnyObjectByType<PlayerDeathController>().Drown();
    }
}
