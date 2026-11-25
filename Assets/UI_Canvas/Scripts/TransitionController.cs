using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    public static TransitionController instance;
    [SerializeField] Animator transition;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        Time.timeScale = 1f;
        SceneManager.sceneLoaded += End;
    }

    void Awake()
    {
        Time.timeScale = 1f;
        Cursor.visible= true;
        Cursor.lockState = CursorLockMode.None;
    }

    void End(Scene scene, LoadSceneMode loaded){
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }
        transition.SetTrigger("Start");
    }
   public void NextLevel()
   {
        //transition.SetTrigger("End");
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        StartCoroutine(LoadScene());    
   }

    IEnumerator LoadScene()
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //transition.SetTrigger("Start");
    }

    public int scene;
    public void Epside(int scene)
    {
        StartCoroutine(LoadEpsiode(scene));
    }

    IEnumerator LoadEpsiode(int index)
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(index);
        transition.SetTrigger("Start");
    }

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
