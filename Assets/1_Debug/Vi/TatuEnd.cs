using UnityEngine;

public class TatuEnd : MonoBehaviour
{
    public PastFutureChange seed;
    bool changed = false;

    void Update()
    {
        if (seed.isChanged != changed)
        {
            changed = seed.isChanged;
            TransitionController.instance.Epside(0);
        }
    }
}
