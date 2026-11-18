using Unity.VisualScripting;
using UnityEngine;

public class Frog : MonoBehaviour
{
    private IFrogState currentState;

    public Transform player;
    private Vector3 target;

    //[Header("Tongue Attack")]
    public TongueController tongue;

    //[Header("Cry Attack")]

    void Start()
    {
        player = PlayerInput.Instance.transform;
        tongue = GetComponent<TongueController>();
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

