
using System.Collections;
using TMPro;
using UnityEngine;

public class GnomeCollectionNotice : MonoBehaviour
{
    public static GnomeCollectionNotice Instance
    {
        get;
        private set;
    }

    [Header("UI")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Configuracao")]
    [SerializeField] private float duration = 3f;

    private Coroutine currentNotification;

    private void Awake()
    {
        Instance = this;

        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ShowCollected(int gnomeId)
    {
        if (notificationPanel == null ||
            messageText == null)
            return;

        int phase = gnomeId <= 7 ? 1 : 2;

        int collected =
            GnomeProgress.GetPhaseCollected(phase);

        if (collected >= 7)
        {
            messageText.text =
                "Colecao completa!\n" +
                "Voce encontrou todos os 7 gnomios desta fase!";
        }
        else
        {
            messageText.text =
                "Gnomio encontrado!\n" +
                $"Voce encontrou {collected} dos 7 " +
                "gnomios desta fase.";
        }

        notificationPanel.SetActive(true);

        if (currentNotification != null)
            StopCoroutine(currentNotification);

        currentNotification =
            StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSecondsRealtime(duration);

        notificationPanel.SetActive(false);
        currentNotification = null;
    }
}
