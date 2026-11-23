using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] TrashManager manager;
    [SerializeField] int life = 3;
    [SerializeField] GameObject[] pileStates;

    public void TakeDamage()
    {
        life--;
        
        if (pileStates[life] != null) 
        {
            pileStates[life].SetActive(false);
        }

        if (life < 1)
        {
            manager.DestroyPile();
        }
    }
    
}
