using UnityEngine;

public class TongueTip : MonoBehaviour
{
    [SerializeField] TongueController tongue;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDeathController>().Pull(transform.position);
        }

        /* else if (other.CompareTag("Trash"))
        {
            other.GetComponent<Trash>().BringDown();
        } */
    }
}
