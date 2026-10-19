using Unity.VisualScripting;
using UnityEngine;

public class ControlUiChange : MonoBehaviour
{
    PlayerInput playerInput;
    [Header("Xbox")]
    public GameObject xbox;
    [Header("Playstation")]
    public GameObject play;
    [Header("Keyboard")]
    public GameObject key;
    void Start()
    {
        playerInput = PlayerInput.Instance;
        playerInput.KeyMapping(ChangeIMG);
        Set(2);
    }

    public void ChangeIMG(PlayerInput.InputType type)
    {
        Set((int)type);
    }
    void OnDisable()
    {
        playerInput.RemoveKeyMapping(ChangeIMG);
    }
    void OnEnable()
    {
        playerInput.KeyMapping(ChangeIMG);
    }
    void Set(int i)
    {
        xbox.SetActive(i==0);
        play.SetActive(i==1);
        key.SetActive(i==2);
    }
}
