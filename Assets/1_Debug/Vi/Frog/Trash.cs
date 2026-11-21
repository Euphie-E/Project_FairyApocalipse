using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] float life = 3;

    public void TakeDamage()
    {
        life--;
    }
    
}
