using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    [SerializeField] Trash trash;
    bool takedDamage = false;

    public void TakeDamage()
    {
        Debug.Log("Chamou tomar dano");
        if (!takedDamage)
        {
            Debug.Log("entro no if");
            trash.TakeDamage();
            Destroy(gameObject);
        }
    }
}
