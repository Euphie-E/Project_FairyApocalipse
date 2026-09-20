using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject pausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        PlayerInput.Instance.AddAction(Pause,6);
    }

    // Update is called once per frame
    public void Pause()
    {

        Time.timeScale = 0f;
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public void BackToGame()
    {
        Time.timeScale = 1;
    }
}
