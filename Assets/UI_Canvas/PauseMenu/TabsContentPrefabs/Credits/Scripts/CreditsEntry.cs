using UnityEngine;

public class CreditsEntry : MonoBehaviour
{
     public CreditsData info;
    public CreditsPanel panel;

    public void OnEntryClicked()
    {
        if (panel == null)
        {

            Debug.LogError("Panel não atribuído no ArchiveEntry!");
            return;
        }
        panel.gameObject.SetActive(true);
        panel.SetData(info);
    }
}
