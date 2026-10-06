using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextColorHighlight : MonoBehaviour
{
    public Toggle myToggle;
    public TextMeshProUGUI tmpText;
    
    public Color colorOn;
    public Color colorOff;

    void Start()
    {
        myToggle = GetComponent<Toggle>();
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (myToggle != null)
        {
            myToggle.onValueChanged.AddListener(OnToggleChanged);
            OnToggleChanged(myToggle.isOn);
        }
    }

    void OnToggleChanged(bool isOn)
    {
        if (tmpText != null)
        {
            if (isOn)
            {
                tmpText.color = colorOn;
            }
            else
            {
                tmpText.color = colorOff;
            }
        }
    }
}
