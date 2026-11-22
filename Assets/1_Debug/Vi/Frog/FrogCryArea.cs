using UnityEngine;

public class FrogCryArea : MonoBehaviour
{
    [SerializeField] Frog frog;
    void OnTriggerEnter(Collider other)
    {
        frog.ChangeState(new FrogCry(frog));
    }

    void OnTriggerExit(Collider other)
    {
        frog.ChangeState(new FrogIdle(frog));
    }
}
