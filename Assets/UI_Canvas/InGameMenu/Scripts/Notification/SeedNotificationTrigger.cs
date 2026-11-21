using UnityEngine;

public class SeedNotificationTrigger : MonoBehaviour
{
    public NotifData[] data;
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
                if(past.CheckSeed() == true)
                {
                    if(past.planted)
                    {
                        NotificationManager.instance.ShowNotif(data[0]);

                        enabled = false;
                        Destroy(this);
                    }
                }
                else
                {
                    NotificationManager.instance.ShowNotif(data[1]);
                }
                
            }
        }
    }
}
