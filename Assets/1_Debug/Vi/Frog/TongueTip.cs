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

        /* else if (other.CompareTag("Weak Point"))
        {
            other.GetComponent<WeakPoint>().BringDown();
        } */

        tongue.OnCollision();
    }
}
