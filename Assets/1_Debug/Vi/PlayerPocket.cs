using System.Collections.Generic;
using UnityEngine;

public class PlayerPocket : MonoBehaviour
{
    static public PlayerPocket Instance;

    public Seed seed;
    public List<ScriptableObject> gnomes;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }


    }

    public bool CollectSeed(Seed seed)
    {
        if (this.seed == null)
        {
            this.seed = seed;
            return true;
        }

        return false;
    }

    public Seed UseSeed()
    {
        if (seed == null)
        {
            Debug.LogError("Tentou plantar a semente sem ter semente");
            return null;
        }

        Seed saveSeed = seed;
        seed = null;
        return saveSeed; 
    }
    
    public void PickSeed(Seed seed)
    {
        if (seed != null)
        {
            Debug.LogError("Tentou pegar a semente sem ter sido plantada");
        }

        this.seed = seed;
    }

    public void CollectGnome(ScriptableObject gnome)
    {
        if (gnomes.Contains(gnome))
        {
            gnomes.Add(gnome);
        }

        else
        {
            Debug.LogError("player coletou 2 gnomos iguais");
        }
    }
}
