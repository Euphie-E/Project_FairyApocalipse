using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField]
    List<Savable> List = new();

    public void Add(Savable s)
    {
        if(s.saveData == null || s.saveData.Key == null)
        {
            Debug.Log("NO SAVABLE DATA");
            #if UNITY_EDITOR
                ShowMe.Create(s,s.gameObject);
            #endif
        }
        List.Add(s);
    }

}
