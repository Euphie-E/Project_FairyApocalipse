using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour
{
    public TMP_Text tittleName, description;
    public Image icon, logo;
    public CreditsData data;

    public void SetData(CreditsData newData)
    {
        data = newData;

        if (data == null)
        {
            Debug.LogWarning("CreditsPanel recebeu data nulo!");
            return;
        }

        tittleName.text = data.name;
        description.text = data.texto;
        icon.sprite = data.image;
        logo.sprite = data.logo;
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
}
