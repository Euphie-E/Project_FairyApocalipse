using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System;
using UnityEngine.UnityConsent;
using Unity.Services.Core.Environments;
using System.Collections.Generic;
using System.Linq;


public class DataEvents
{
    public bool isInitialized = false;
    public enum Statistics {
        JumpTotal,
        Jump,
        TimeTravelTotal,
        TimeTravel,
        DeathTotal,
        Death,
        GDuration,
        CheckPoint
    };

    string user;
    public int index;
    int playerCT = 0;
    public async void Run(string environment)
    {
        if(environment == "disable") return;
        try
        {
            // Initialize fundamental Unity Gaming Services
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName(environment));
            // Explicitly opt-in / start data collection for this player
            //AnalyticsService.Instance.StartDataCollection();
            EndUserConsent.SetConsentState(new ConsentState 
            { 
                AnalyticsIntent = ConsentStatus.Granted, 
                AdsIntent = ConsentStatus.Denied 
            });
            isInitialized = true;
            
            Debug.Log("Unity Analytics 6.3 Successfully Initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError($"UGS Initialization Failed: {e.Message}");
        }
        user = SystemInfo.deviceUniqueIdentifier;
    }
    public int AddPlayerCT(int start)
    {
        playerCT = start+1;
        return AddPlayerCT();
    }
    public int AddPlayerCT()
    {
        user = playerCT.ToString();
        playerCT++;
        return playerCT-1;
    }
    public void Add(Statistics statistics,int value)
    {
        if(!isInitialized) return;
        CustomEvent myEvent = new CustomEvent(statistics.ToString())
        {
            {statistics.ToString(), value},
            {"UserID",user},
            {"SceneIndex",index}
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
    public void Add(Statistics statistics, float value, int id = -1)
    {
        if(!isInitialized) return;
        CustomEvent myEvent = new CustomEvent(statistics.ToString())
        {
            {statistics.ToString(), value},
            {"UserID",user},
            {"SceneIndex",index}
        };
        if(id != -1) myEvent.Add("CheckpointIndex",id);
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
}

[Serializable]
public class PlayerDataGroup
{
    public PlayerData[] group;
    public int JumpTotal;
    public int DeathTotal;
    public int TimeTravelTotal;
    public PlayerDataGroup(int size)
    {
        group = new PlayerData[size];
        for(int i = 0; i < size; i++)
        {
            group[i] = new PlayerData();
        }
    }
}

[Serializable]
public class PlayerData
{
    public int index = -1;
    public int Jump;
    public int TimeTravel;
    public int Death;
    public double Duration;
    public double[] CheckPoints = new double[4];
    public List<Save> LevelData = new();

    public Save Load(Savable.Types ClassName)
    {
        return LevelData.Find(item => item.Key == ClassName);
    }


}

[Serializable]
public class Save
{
    public Savable.Types Key;
    public int ID = -1;
    public List<KeyValue> Values = new();
    public Save(Savable.Types Name, int ID)
    {
        Key = Name;
        this.ID = ID;
    }
    public void Add(KeyValue value)
    {
        if(value == null || Values == null) return;
        KeyValue temp = Values.FirstOrDefault(k => k/value);
        if(temp == null)
        {
            Values.Add(value);  
        }
        else
        {
            temp.value = value.value;
        }
    }
    public void Sort()
    {
        Values.Sort((p1, p2) => p1*p2);
    }
    public int Get(string key)
    {
        KeyValue a = Values.FirstOrDefault(k => k.key == key);
        return a != null ? a.value : -1;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Save temp = (Save)obj;
        return this.Key == temp.Key && this.ID == temp.ID;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Key, ID);
    }
}

[Serializable]
public class KeyValue
{
    public string key;
    public int value;
    public static bool operator /(KeyValue p1,KeyValue p2)
    {
        if(p1 == null || p2 == null) return false;
        return p1.key == p2.key;
    }
    public static int operator *(KeyValue p1,KeyValue p2)
    {
        return p1.key.CompareTo(p2.key);
    }
    public override string ToString()
    {
        return "Key: "+key+" | Value: "+value;
    }
}

[Serializable]
public abstract class Savable: MonoBehaviour
{
    public enum Types
    {
        Checkpoint,
        PastFutureChange,
        Collectible,
        test
    }
    public abstract Save Save();
    public abstract void Load(List<Save> save);
    public Save saveData;
    public static Save GetSave(List<Save> save,Types types, int id)
    {
        return save.FirstOrDefault(x => x.Key == types && x.ID == id);
    }
}





