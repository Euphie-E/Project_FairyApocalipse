using UnityEngine;

public class SeedNotificationTrigger : MonoBehaviour
{
    public NotifData data;
    public PastFutureChange past;

    void Start()
    {
        past = GetComponentInParent<PastFutureChange>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(NotificationManager.instance != null)
            {   
                if(!past.CheckSeed())
                {
                    NotificationManager.instance.ShowNotif(data);
                }
                else
                {
                    enabled = false;
                    Destroy(this);
                }
            }
        }
    }
}
