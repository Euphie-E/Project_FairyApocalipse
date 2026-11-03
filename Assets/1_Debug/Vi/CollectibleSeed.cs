using System.Collections.Generic;
using UnityEngine;

public class CollectibleSeed : Collectible
{
    [SerializeField] private Seed seed;

    protected override void Collect()
    {
        if (PlayerPocket.Instance.CollectSeed(seed))
        {
            base.Collect();
        }
    }

    protected override void CollectEffect()
    {
        FairyController.Instance.PickSeed(this);
    }

    protected override void CollectInfo()
    {
        base.CollectInfo();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Collected)
        {
            Collect();
        }
    }

    public bool isHolding = false;
    public void SaveState(bool holding)
    {
        isHolding = holding;
    }

    public override Save Save()
    {
        var temp = base.Save();
        temp.Add(new KeyValue{key = "isHolding", value = isHolding?1:0});
        return temp;
    }
    public override void Load(List<Save> save)
    {
        saveData = GetSave(save,Types.Collectible,id);
        if(saveData == null) 
            return;
        int b = saveData.Get("Collected");
        int h = saveData.Get("isHolding");
        if(b == 1) {
            if(h == 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Collected = true;
                CollectEffect();
                Collect();
            }
        }
    }
    public void SaveMe()
    {
        LevelManager.Instance?.SaveMe(this);
    }
    

}
