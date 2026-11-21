using UnityEngine;

public class ArchiveUnlock : MonoBehaviour
{
    public GameObject[] entries;
    void Start()
    {
        
    }

    void Awake()
    {
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].SetActive(false);
        }

        entries[0].SetActive(true); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
