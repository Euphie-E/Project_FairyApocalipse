using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchivePanel : MonoBehaviour
{

    public TMP_Text tittleName, description;
    public Image icon;
    public ArchiveData data;

    public void SetData(ArchiveData newData)
    {
        data = newData;

        if (data == null)
        {
            Debug.LogWarning("ArchivePanel recebeu data nulo!");
            return;
        }

        tittleName.text = data.tName;
        description.text = data.description;
        icon.sprite = data.icon;
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}
