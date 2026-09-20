using UnityEngine;
using UnityEngine.UI;

public class ToggleHighlight : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image  imageToKeepFocusActive;
        
    private void Reset()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (imageToKeepFocusActive == null) return;

        if (toggle.isOn)
        {
            SetAlpha(1f);
        }
        else
        {
            SetAlpha(0f);
        }
    }
    
    void SetAlpha(float alphaValue)
    {
        if (imageToKeepFocusActive != null)
        {
            Color corAtual = imageToKeepFocusActive.color;
            corAtual.a = alphaValue; 
            imageToKeepFocusActive.color = corAtual;
        }
    }
}
