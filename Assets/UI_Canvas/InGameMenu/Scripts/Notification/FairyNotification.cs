using UnityEngine;

public class FairyNotification : MonoBehaviour
{
    public NotifData data;
    public TimeTravel timeTravel;
    void Start()
    {
        timeTravel = FindFirstObjectByType<TimeTravel>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(NotificationManager.instance != null)
            {   
                if(!timeTravel.hasFairy)
                {
                    NotificationManager.instance.ShowNotif(data);
                }
                else if(timeTravel.hasFairy)
                {
                    enabled = false;
                    Destroy(this);
                }
            }
        }
    }
}
