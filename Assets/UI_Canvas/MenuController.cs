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
                pausePanel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                pausePanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            
        }

        TimeTravelIcon();
        
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InitialMenu()
    {
        SceneManager.LoadScene(0);
    }
}
