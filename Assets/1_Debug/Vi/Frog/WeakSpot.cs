using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    [SerializeField] Trash trash;
    [SerializeField] GameObject trashPile;
    bool takedDamage = false;

    public void TakeDamage()
    {
        if (!takedDamage)
        {
            trash.TakeDamage();
        }
    }
}
