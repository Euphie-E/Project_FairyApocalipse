using Unity.VisualScripting;
using UnityEngine;

public class FrogLick : IFrogState
{
    Frog frog;
    TongueController tongue;
    float waitTime;

    public FrogLick(Frog frog)
    {
        this.frog = frog;
        tongue = frog.tongue;
        //waitTime = tongue.inter;
    }

    public void Enter()
    {
        
    }

    public void Process()
    {
        if (!frog.tongue.isLicking) return;

        if (waitTime > 0 && !tongue.isLicking)
        {
            waitTime -= Time.deltaTime;
        }

        else 
        {
            Lick();
        }
    }

    public void Lick()
    {
        tongue.StartLick();
        waitTime = tongue.lickInterval;
    }

    public void Exit()
    {
        
    }
}
