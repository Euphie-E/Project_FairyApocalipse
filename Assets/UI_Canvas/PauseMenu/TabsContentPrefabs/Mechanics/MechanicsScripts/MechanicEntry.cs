using UnityEngine;

public class MechanicEntry : MonoBehaviour
{
    public MechanicData mInfo;
    public MechanicPanel panel;

    public void OnEntryClicked()
    {
        if (panel == null)
        {

            Debug.LogError("Panel não atribuído no ArchiveEntry!");
            return;
        }
        panel.SetData(mInfo);
        panel.gameObject.SetActive(true);
       
    }
}
