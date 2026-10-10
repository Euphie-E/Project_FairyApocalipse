using UnityEngine;

public class ArchiveEntry : MonoBehaviour
{
    public ArchiveData info;
    public ArchivePanel panel;

    // Chame este método no OnClick do Button (via Inspector)
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
