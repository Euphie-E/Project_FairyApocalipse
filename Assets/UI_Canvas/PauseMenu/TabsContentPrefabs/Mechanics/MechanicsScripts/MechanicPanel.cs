using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MechanicPanel : MonoBehaviour
{
    public TMP_Text tittleName, description, keyKeyboard;
    public UISpriteAnimation gif;
    public MechanicData data;
    public Image keyControl;

    public void SetData(MechanicData newData)
{
    data = newData;

    if (data == null)
    {
        Debug.LogWarning("MechanicPanel recebeu data nulo!");
        return;
    }

    tittleName.text = data.tName;
    description.text = data.description;
    keyKeyboard.text = data.keyKeyboard;
    keyControl.sprite = data.keyControl;

    if (gif != null)
    {
        gif.m_SpriteArray = data.gif;

        if (data.gif != null && data.gif.Length > 0)
            gif.m_Image.sprite = data.gif[0];
        else
            gif.m_Image.sprite = null;

        gif.Play();
    }
}

    public void OnBackButtonClick()
    {
        if (gif != null)
            gif.Stop();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (data != null && gif != null)
            gif.Play();
    }
}