using UnityEngine;

public class CollectibleSeed : Collectible
{
    [SerializeField] Seed seed;

    void Awake()
    {
        
    }

    protected override void Collect()
    {
        PlayerPocket.Instance.CollectSeed(seed);
        base.Collect();
    }

    protected override void CollectEffect()
    {
        base.CollectEffect();
    }

    protected override void CollectInfo()
    {
        base.CollectInfo();
    }
}
