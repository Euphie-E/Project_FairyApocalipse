using UnityEngine;

public class ListenerExample : MonoBehaviour
{
    void Start()
    {
        BossExample.Self.AddListener(BossExample.BossEvents.OnDeath, OnDeath);
    }

    public void OnDeath()
    {
        Debug.Log("ListenerExample: OnDeath");
    }
}
