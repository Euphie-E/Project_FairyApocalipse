using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenuController : MonoBehaviour
{
    public void Play()
    {
        Debug.Log("Jogar");
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
