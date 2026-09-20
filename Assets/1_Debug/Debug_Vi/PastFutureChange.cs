using NUnit.Framework;
using UnityEngine;

public class PastFutureChange : Interactable
{
    [SerializeField] GameObject[] pastObject;
    [SerializeField] GameObject[] pastChangedObject;
    [SerializeField] GameObject[] futureObject;
    [SerializeField] GameObject[] futureChangedObject;
    [SerializeField] private bool isChanged = false;

    [SerializeField] private bool interactInFuture = false;
    [SerializeField] private bool isReversible = false;


    void Start()
    {
        DisableAll(pastChangedObject);
        DisableAll(futureChangedObject);
        ActiveAll(pastObject);
        ActiveAll(futureObject);
    }

    protected override void Interact(InteractController player, GameObject attach)
    {
        Debug.Log("Interagi");
        if (player.GetComponent<TimeTravel>().isTravelling ^ interactInFuture) // se for pra funcionar no passado funciona no passado, se for pra funcionar no futuro funciona no futuro
        {
            Change();
        }
    }

    public void Change()
    {
        if (isReversible || !isChanged)
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
