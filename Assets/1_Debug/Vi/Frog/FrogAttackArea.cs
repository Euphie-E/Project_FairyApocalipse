using UnityEngine;

public class FrogAttackArea : MonoBehaviour
{
    [SerializeField] Frog frog;
    void OnTriggerEnter(Collider other)
    {
        frog.ChangeState(new FrogLick(frog));
    }

    void OnTriggerExit(Collider other)
    {
        frog.ChangeState(new FrogIdle(frog));
    }
}
