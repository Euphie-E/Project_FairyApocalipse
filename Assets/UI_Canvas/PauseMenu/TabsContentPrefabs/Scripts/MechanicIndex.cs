using UnityEngine;

public class MechanicIndex : MonoBehaviour
{
    public int index;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MenuController.instance.EntryMechanic(index);
        }
    }
}
