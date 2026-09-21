using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pausePanel.SetActive(false);
        PlayerInput.Instance.AddAction(Pause,6);
    }

    public void Pause()
    {
        // Alterna entre pausado e despausado
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    public void BackToGame()
    {
        isPaused = false;

        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}
