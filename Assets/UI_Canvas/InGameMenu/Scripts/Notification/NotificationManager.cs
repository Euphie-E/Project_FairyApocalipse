
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    public GameObject notificationParent;
    public TMP_Text notficationText;
    
    private CanvasGroup notificationCanvasGroup;
    private Queue<NotifData> notifQueue = new Queue<NotifData>();
    private bool isDisplayingNotif = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        notificationCanvasGroup = notificationParent.GetComponent<CanvasGroup>();
        notificationCanvasGroup.alpha = 0;
    }

    public void ShowNotif(NotifData data)
    {
        notifQueue.Enqueue(data);
        if(!isDisplayingNotif)
        {
            StartCoroutine(DisplayNotification());
        }
    }

    private IEnumerator DisplayNotification()
    {
        isDisplayingNotif = true;
        while (notifQueue.Count > 0)
        {
            NotifData data = notifQueue.Dequeue();
            notficationText.text = data.notifText;

            yield return StartCoroutine(Fade(notificationCanvasGroup, true, data.fade)); //FadeIN
            yield return new WaitForSeconds(data.duration); //Display
            yield return StartCoroutine(Fade(notificationCanvasGroup, false, data.fade)); //FadeOUT
        }
        isDisplayingNotif = false;
    }

    public IEnumerator Fade(CanvasGroup cG, bool fade, float duration)
    {
        float targetAlpha = fade ? 1f : 0f;
        float initialAlpha = cG.alpha;
        float elapseTime = 0f;

        while(elapseTime < duration)
        {
            elapseTime += Time.deltaTime;
            cG.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapseTime / duration);
            yield return null;
        }   
        cG.alpha = targetAlpha; 
    }
}
