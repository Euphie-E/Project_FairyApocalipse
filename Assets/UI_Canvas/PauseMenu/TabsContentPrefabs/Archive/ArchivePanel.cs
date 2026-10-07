using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchivePanel : MonoBehaviour
{
    public TMP_Text tName, description;
    public Image icon;
    public ArchiveData data;

    void Start()
    {
        tName.text = data.tName;
        description.text = data.description;
        icon.sprite = data.icon;
    }

    void OnBackButtonClick() 
    { 
        this.gameObject.SetActive(false);
    }
}
