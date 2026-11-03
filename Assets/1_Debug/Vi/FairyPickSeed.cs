using UnityEngine;

public class FairyPickSeed : MonoBehaviour
{
    // Gambiarra pq o código do pai tem que ta no filho aff
    public void PickSeed()
    {
        FairyController.Instance.GrabSeed();
    }
}
