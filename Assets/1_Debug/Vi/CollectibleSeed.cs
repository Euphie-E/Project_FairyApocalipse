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
        FairyController.Instance.PickSeed(transform);
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


}
