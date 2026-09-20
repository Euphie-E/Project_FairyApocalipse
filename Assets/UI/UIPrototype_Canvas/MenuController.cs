using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject pausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            Time.timeScale = 0f;
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    public void BackToGame()
    {
        Time.timeScale = 1;
    }
}
