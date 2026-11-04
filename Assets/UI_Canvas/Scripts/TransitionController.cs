using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class TransitionController : MonoBehaviour
{
    private static TransitionController instance;
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
        SceneManager.sceneLoaded += End;
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

    public void Quit()
    {
        Debug.Log("Fechou");
        Application.Quit();
    }
}
