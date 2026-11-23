using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using GCamarada;

public class LevelManager : MonoBehaviour
{
    static LevelManager Instance;
    private void Start()
    {
        AudioManager.I.PlayFullMusic();
    }
    public static LevelManager Get()
    {
        if(Instance == null) return null;
        return Instance;
    }
    public GameObject Player;
    public void SetPlayer(GameObject player)
    {
        Player = player;
    }

    public void SetLastCheckpoint(int id)
    {
        Checkpoint checkpoint = Checkpoints
            .OfType<Checkpoint>()
            .FirstOrDefault(c => c.index == id);

        if (checkpoint == null)
        {
            Debug.LogWarning(
                $"Checkpoint {id} nao encontrado!"
            );
            return;
        }

        // Substitui o ultimo checkpoint.
        lastCKP = id;

        // Incrementa a ordem de ativacao.
        checkpointActivationOrder++;

        // Atualiza os dados do checkpoint.
        checkpoint.RegisterActivation(
            checkpointActivationOrder
        );

        // Atualiza os dados em memoria.
        SaveMe(checkpoint);

        Debug.Log($"Ultimo checkpoint atualizado: {id}");
    }

    public Checkpoint GetCheckpoint(int index)
    {
        return (Checkpoint)Checkpoints[index];
    }
    public Checkpoint GetLastCheckpoint()
    {
        return Checkpoints
            .OfType<Checkpoint>()
            .FirstOrDefault(c => c.index == lastCKP);
    }
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
    int lastCKP = 0;
    int checkpointActivationOrder = 0;
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
                    Checkpoints.Sort((P1, P2) => P1.saveData.ID.CompareTo(P2.saveData.ID));
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
        if (save == null)
            return;

        Saves = new List<Save>(save);

        foreach (var item in List)
        {
            item.Load(save);
        }

        int ck = -1;
        Save sv = null;

        foreach (var item in save)
        {
            if (item.Key != Savable.Types.Checkpoint)
                continue;

            int count = item.Get("Count");

            if (count > ck)
            {
                ck = count;
                sv = item;
            }
        }

        // Recupera a ordem da ultima ativacao.
        checkpointActivationOrder = Mathf.Max(0, ck);

        if (sv == null || ck == -1)
            return;

        // Recupera o ID do ultimo checkpoint.
        lastCKP = sv.ID;

        Checkpoint checkpoint = GetLastCheckpoint();

        if (checkpoint != null && Player != null)
        {
            PlayerMovement movement =
                Player.GetComponent<PlayerMovement>();

            if (movement != null)
            {
                movement.TeleportTo(checkpoint.transform);
            }
        }
    }

    public void SaveMe(Savable s)
    {
        Save updated = s.Save();

        // Remove versoes anteriores do mesmo registro.
        Saves.RemoveAll(k =>
            k.Key == updated.Key &&
            k.ID == updated.ID
        );

        Saves.Add(updated);
    }

    public List<Save> Save()
    {
        Debug.Log("Saving");

        foreach (var item in List)
        {
            if (!item.enabled)
                continue;

            SaveMe(item);
        }

        return new List<Save>(Saves);
    }

}


