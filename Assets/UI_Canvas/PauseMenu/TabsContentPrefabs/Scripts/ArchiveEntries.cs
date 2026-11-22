using UnityEngine;

public class ArchiveEntries : MonoBehaviour
{
    public GameObject[] archiveEntries;
    public int entriesFilled = 1;
    void Start()
    {
        
    }

    void Awake()
    {
        for (int i = entriesFilled; i < archiveEntries.Length; i++)
        {
            archiveEntries[i].SetActive(false);
        }
    }

    void Update()
    {
        
    }

    public void FillArchiveEntry(int i)
    {
        archiveEntries[i].SetActive(true);
    }

    public void FillMechanicEntry(int i)
    {
        archiveEntries[i].SetActive(true);
    }
}
