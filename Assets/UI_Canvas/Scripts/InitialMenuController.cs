using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenuController : MonoBehaviour
{
    public TransitionController transition;
    public void Start()
    {
        transition = FindFirstObjectByType<TransitionController>();
    }
    public void Play()
    {
        Debug.Log("Jogar");
        transition.NextLevel();
    }

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
