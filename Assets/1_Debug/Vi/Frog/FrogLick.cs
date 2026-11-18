using Unity.VisualScripting;
using UnityEngine;

public class FrogLick : MonoBehaviour, IFrogState
{
    Frog frog;
    float waitTime;

    public FrogLick(Frog frog)
    {
        this.frog = frog;
        waitTime = frog.intervalAttack;
    }

    public void Enter()
    {
        
    }

    public void Process()
    {
        if (waitTime > 0 )
        {
            waitTime -= Time.deltaTime;
        }
    }

    public void Exit()
    {
        
    }
}
