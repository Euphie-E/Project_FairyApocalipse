using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    private static TransitionController instance;
    [SerializeField] Animator transition;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

   public void NextLevel()
   {
        StartCoroutine(LoadScene());    
   }

    IEnumerator LoadScene()
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transition.SetTrigger("Start");
    }

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
