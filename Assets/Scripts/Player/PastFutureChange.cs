using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Seed plantedSeed = null;
    [SerializeField] private int id = -1;

    void Start()
    {
        DisableAll(pastChangedObject);
        DisableAll(futureChangedObject);
        ActiveAll(pastObject);
        ActiveAll(futureObject);
        saveData = new Save(Types.PastFutureChange,id);
        LevelManager.Instance?.Add(this);
    }

    protected override void Interact(InteractController player, GameObject attach)
    {
        Debug.Log("interagi");
        Debug.Log("Check seed:" + CheckSeed());
        Debug.Log("is changed:" + isChanged);
        if (player.GetComponent<TimeTravel>().isTravelling ^ interactInFuture && (CheckSeed() ^ isChanged)) // se for pra funcionar no passado funciona no passado, se for pra funcionar no futuro funciona no futuro
        {
            Change();
        }
    }

    private bool CheckSeed()
    {
        if (seeds.Contains(PlayerPocket.Instance.seed))
        {
            return true;
        }

        return false;
    }

    public void Change()
    {
        if (isReversible || !isChanged)
        {
            if (isChanged)
            {
                PlayerPocket.Instance.PickSeed(plantedSeed);
                plantedSeed = null;
            }
            else
            {
                plantedSeed = PlayerPocket.Instance.UseSeed();
            }

            isChanged = !isChanged;

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
    public override Save Save()
    {
        if(saveData == null) return null;
        saveData.Add(new KeyValue{key = "isChanged",value = isChanged?1:0});
        saveData.Add(new KeyValue{key = "seedID",value = plantedSeed != null ? plantedSeed.seedIndex: -1});
        return saveData;
    }
    public override void Load(List<Save> save)
    {
        saveData = GetSave(save,Types.PastFutureChange,id);
        if(saveData == null) 
            return;
        int b = saveData.Get("isChanged");
        if(b == 1) {
            isChanged = true;
            plantedSeed = LevelManager.Instance?.GetSeed(saveData.Get("seedID"));
            DisableAll(pastObject);
            DisableAll(futureObject);
            ActiveAll(pastChangedObject);
            ActiveAll(futureChangedObject);
        }
    }
}