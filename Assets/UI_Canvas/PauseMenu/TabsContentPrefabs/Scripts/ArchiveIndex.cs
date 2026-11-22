using UnityEngine;

public class ArchiveIndex : MonoBehaviour
{
    public int index;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MenuController.instance.EntryArchive(index);
            enabled = false;
            Destroy(this);
        }
    }
}
