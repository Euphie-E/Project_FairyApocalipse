using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public TimeTravel timeTravel;
    
    public Image iconTimeTravel;
    public Sprite iconTimeAvalible, iconTimeUnavalible;
    public GameObject blockTimeTravel;
    public GameObject pausePanel;

    public ArchiveEntries archiveEntries;
    public int indexArchive, indexMechanic;

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
    void OnEnable()
    {
        PlayerInput.Instance?.AddAction(Pause, 6);
    }
    void OnDisable()
    {
        PlayerInput.Instance?.RemoveAction(Pause, 6);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                UnPauseEffect();
            }
            else
            {
                PauseEffect();
            }
            
        }

        TimeTravelIcon();
        
    }

    public void PauseEffect()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnPauseEffect()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Pause(InputAction.CallbackContext ctx)
    {
        return;
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

    // public void CloseButton()
    // {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }

    public void InitialMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void EntryArchive(int i)
    {
        indexArchive = i;
        archiveEntries.FillEntry(i);
    }

    public void EntryMechanic(int i)
    {
        indexMechanic = i;
    }
}
