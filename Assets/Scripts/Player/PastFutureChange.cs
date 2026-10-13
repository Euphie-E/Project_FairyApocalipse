using NUnit.Framework;
using Unity.VisualScripting;
using System.Collections.Generic;
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
    [SerializeField] private List<Seed> seeds;

    void Start()
    {
        DisableAll(pastChangedObject);
        DisableAll(futureChangedObject);
        ActiveAll(pastObject);
        ActiveAll(futureObject);
    }

    protected override void Interact(InteractController player, GameObject attach)
    {
        if (player.GetComponent<TimeTravel>().isTravelling ^ interactInFuture && CheckSeeds(player.seeds)) // se for pra funcionar no passado funciona no passado, se for pra funcionar no futuro funciona no futuro
        {
            Change();
        }
    }

    private bool CheckSeeds(List<Seed> seeds)
    {
        //encontrar a seed em comum e remover ela do player
        return false;
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
