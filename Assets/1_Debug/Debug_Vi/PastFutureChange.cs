using UnityEngine;

public class PastFutureChange : MonoBehaviour
{
    [SerializeField] GameObject[] pastObject;
    [SerializeField] GameObject[] pastChangedObject;
    [SerializeField] GameObject[] futureObject;
    [SerializeField] GameObject[] futureChangedObject;
    [SerializeField] bool isChanged = false;


    void Start()
    {
        DisableAll(pastChangedObject);
        DisableAll(futureChangedObject);
        ActiveAll(pastObject);
        ActiveAll(futureObject);
    }

    void Update()
    {
        if (isChanged)
        {
            Change();
        }
    }

    public void Change()
    {
        if (/* ! */isChanged)
        {
            Debug.Log("foi");
            isChanged = true;
            DisableAll(pastObject);
            DisableAll(futureObject);
            ActiveAll(pastChangedObject);
            ActiveAll(futureChangedObject);
        }
    }

    private void ActiveAll(GameObject[] objects)
    {
        if (objects.Length != 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(true);
            }
        }
    }

    private void DisableAll(GameObject[] objects)
    {
        if (objects.Length != 0)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
        }
    }
}
