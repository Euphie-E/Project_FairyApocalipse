using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index = 0;
    void Start()
    {
        PlayerInput.Instance?.AddCheckpoint(this.gameObject,index);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeathController playerDeath = other.GetComponent<PlayerDeathController>();
            playerDeath.NewCheckpoint(index, gameObject);
        }
    }
}
