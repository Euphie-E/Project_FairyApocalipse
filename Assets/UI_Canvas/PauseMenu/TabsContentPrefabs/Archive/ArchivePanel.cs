using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchivePanel : MonoBehaviour
{

    public TMP_Text tName, description;
    public Image icon;
    public ArchiveEntry data;

    void Start()
    {
        data = FindFirstObjectByType<ArchiveEntry>();
        tName.text = data.info.tName;
        description.text = data.info.description;
        icon.sprite = data.info.icon;
    }

    void OnBackButtonClick() 
    { 
        this.gameObject.SetActive(false);
    }
}
