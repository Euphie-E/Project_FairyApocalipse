using UnityEngine;

public class FrogCry : IFrogState
{
    Frog frog;
    TongueController tongue;
    EartquakeController cry;

    public FrogCry(Frog frog)
    {
        this.frog = frog;
        tongue = frog.tongue;
        cry = frog.cry;
    }

    public void Enter()
    {
        cry.StartAutoEarthquakes();
    }

    public void Process()
    {

    }

    public void Exit()
    {
        cry.StopEarthquakes();
    }
}
