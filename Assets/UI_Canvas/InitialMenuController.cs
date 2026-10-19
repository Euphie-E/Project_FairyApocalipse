using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenuController : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Jogar");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
