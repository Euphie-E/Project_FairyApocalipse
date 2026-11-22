using UnityEngine;

public class FrogLickArea : MonoBehaviour
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
