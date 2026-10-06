using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index = 0;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeathController playerDeath = other.GetComponent<PlayerDeathController>();
            playerDeath.NewCheckpoint(index, gameObject);
        }
    }
}
