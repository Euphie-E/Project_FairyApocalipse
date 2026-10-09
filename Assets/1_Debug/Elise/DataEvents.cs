using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System;
using UnityEngine.UnityConsent;
using Unity.Services.Core.Environments;
using System.Collections.Generic;


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
        CheckPoints
    };

    string user;
    public int index;
    int playerCT = 0;
    public async void Run(string environment)
    {
        try
        {
            // Initialize fundamental Unity Gaming Services
            await UnityServices.InitializeAsync();
            new InitializationOptions().SetEnvironmentName(environment);
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
}
