using UnityEngine;

public class TongueTip : MonoBehaviour
{
    [SerializeField] TongueController tongue;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathController>().Pull(tongue.transform.position);
        }

        else if (other.CompareTag("Weak Spot"))
        {
            Debug.Log("Entrou");
            other.GetComponent<WeakSpot>().TakeDamage();
        }

        tongue.OnCollision();
    }
}
