using UnityEngine;

public class TrashManager : MonoBehaviour
{
    int piles = 3;

    public void DestroyPile()
    {
        piles--;

        if (piles <= 0)
        {
            TransitionController.instance.Epside(0);
        }
    }
}
