using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GameObject Player;
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
    List<Savable> Checkpoints = new();
    List<Savable> PastFutureChange = new();
    [SerializeField]
    List<Seed> Seeds = new();
    List<Save> Saves = new();

    public void Add(Savable s)
    {
        if(s.saveData == null || s.saveData.ID == -1)
        {
            Debug.Log("NO SAVABLE DATA");
            Error(s,"No Save Data",s.gameObject);
        }
        if (!List.Contains(s))
        {
            switch (s.saveData.Key)
            {
                case Savable.Types.Checkpoint:
                    if(Checkpoints.Any(id => id.saveData.ID == s.saveData.ID))
                    {
                        Error(s,"Duplicate ID: "+ s.saveData.ID,s.gameObject);
                        return;
                    }
                    Checkpoints.Add(s);
                    break;
                case Savable.Types.PastFutureChange:
                    if(PastFutureChange.Any(id => id.saveData.ID == s.saveData.ID))
                    {
                        Error(s,"Duplicate ID: "+ s.saveData.ID,s.gameObject);
                        return;
                    }
                    PastFutureChange.Add(s);
                    break;
                default:
                    break;
            }
            List.Add(s);
            
        }
    }
    public void Remove(Savable s)
    {
        if (List.Contains(s))
        {
            switch (s.saveData.Key)
            {
                case Savable.Types.Checkpoint:
                    Checkpoints.Remove(s);
                    break;
                case Savable.Types.PastFutureChange:
                    PastFutureChange.Remove(s);
                    break;
                default:
                    break;
            }
            List.Remove(s);
        }  
    }
    void Error(Object code,string msg, GameObject obj = null)
    {
        #if UNITY_EDITOR
            ShowMe.Create(this,msg,obj);
        #endif
    }
    public Seed GetSeed(int index)
    {
        return Seeds.FirstOrDefault(k => k.seedIndex == index);
    }
    public void Load(List<Save> save)
    {
        int ck = -1;
        Save sv = null;
        foreach (var item in List)
        {
            item.Load(save);
        }
        foreach (var item in save)
        {
            if(item.Key == Savable.Types.Checkpoint && item.Get("Count") > ck)
            {
                ck = item.Get("Count");
                sv = item;
            }
        }
        if(sv != null && ck != -1) 
            Player.GetComponent<PlayerMovement>().TeleportTo(Checkpoints.FirstOrDefault(k => k.saveData.ID == sv.ID)?.transform);
    }
    public List<Save> Save()
    {
        Debug.Log("Saving");
        List<Save> temp = new();
        temp.AddRange(Saves);
        foreach (var item in List)
        {
            if(item.enabled == false) continue;
            Saves.Add(item.Save());
        }
        return Saves;
    }
    public void SaveMe(Savable s)
    {
        Save temp = Saves.FirstOrDefault(k => k.Key == s.saveData.Key && k.ID == s.saveData.ID);
        if(temp == null)
            Saves.Add(s.Save());
        else{
            temp = s.Save();
        }
    }
}

