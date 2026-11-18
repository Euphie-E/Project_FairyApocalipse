using Unity.VisualScripting;
using UnityEngine;

public class Frog : MonoBehaviour
{
    private IFrogState currentState;

    private Transform player;
    private Vector3 target;

    [Header("Tongue Attack")]
    public Tongue tongueBase;
    public float attackTime;
    public float returnTime;
    public float intervalAttack;

    //[Header("Cry Attack")]

    void Start()
    {
        player = PlayerInput.Instance.transform;
        ChangeState(new FrogIdle(this));
    }

    public void ChangeState(IFrogState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    void Update()
    {
        currentState.Process();
    }
}

