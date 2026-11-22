using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenuController : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Jogar");
        
    }

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
