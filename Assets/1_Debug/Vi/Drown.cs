using UnityEngine;

public class Drown : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        FindAnyObjectByType<PlayerDeathController>().Drown();
    }
}
