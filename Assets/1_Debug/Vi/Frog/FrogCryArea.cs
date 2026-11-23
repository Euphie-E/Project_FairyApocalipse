using UnityEngine;

public class FrogCryArea : MonoBehaviour
{
    [SerializeField] Frog frog;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            frog.ChangeState(new FrogCry(frog));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            frog.ChangeState(new FrogIdle(frog));
        }
    }
}
