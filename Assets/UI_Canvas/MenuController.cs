using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private MenuController instance;
    public TimeTravel timeTravel;
    
    public Image iconTimeTravel;
    public Sprite iconTimeAvalible, iconTimeUnavalible;
    public GameObject blockTimeTravel;
    public GameObject pausePanel;

    void Start()
    {
        Time.timeScale = 1f;
        
        pausePanel.SetActive(false);
        
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        timeTravel = FindFirstObjectByType<TimeTravel>();
        iconTimeTravel.sprite = iconTimeAvalible;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        TimeTravelIcon();
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void TimeTravelIcon()
    {
        if(timeTravel.isTravelling)
        {
            iconTimeTravel.sprite = iconTimeUnavalible;
        }
        else
        {
            iconTimeTravel.sprite = iconTimeAvalible;
        }

        if(timeTravel.TravelBlocked)
        {
            blockTimeTravel.SetActive(true);
        }
        else
        {
            blockTimeTravel.SetActive(false);
        }
    }

    public void CloseButton()
    {
        ResumeGame();
    }

    public void InitialMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}