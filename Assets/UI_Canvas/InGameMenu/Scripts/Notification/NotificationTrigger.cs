using UnityEngine;

public class NotificationTrigger : MonoBehaviour
{
    public NotifData data;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(NotificationManager.instance != null)
            {
                NotificationManager.instance.ShowNotif(data);
            }
            enabled = false;
        }
    }
}
