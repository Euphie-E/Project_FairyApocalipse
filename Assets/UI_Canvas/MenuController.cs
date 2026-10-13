using UnityEngine;

public class MenuController : MonoBehaviour
{
    private MenuController instance;
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

    }

    // Update is called once per frame
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
        
    }
}
