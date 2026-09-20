using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class canvas : MonoBehaviour
{
    public GameObject canvasMenu;
    void Start()
    {
        canvasMenu.SetActive(false);
    }

    void Update()
    {
        if (PlayerInput.Instance.menuActive.WasCompletedThisFrame())
        {
            canvasMenu.SetActive(!canvasMenu.activeSelf);
        }
    }
}
