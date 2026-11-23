using UnityEngine;

public class FrogLickArea : MonoBehaviour
{
    [SerializeField] Frog frog;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {    
            frog.ChangeState(new FrogLick(frog));
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
