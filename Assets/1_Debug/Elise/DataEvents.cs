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
    public void AddPlayerCT()
    {
        user = playerCT.ToString();
        playerCT++;
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

public class PlayerDataGroup
{
    public PlayerData[] group;
    public PlayerDataGroup(int size)
    {
        group = new PlayerData[size];
        for(int i = 0; i < size; i++)
        {
            group[i] = new PlayerData();
        }
    }
}

public class PlayerData
{
    public int index;
    public static int JumpTotal;
    public int Jump;
    public static int TimeTravelTotal;
    public int TimeTravel;
    public static int DeathTotal;
    public int Death;
    public float Duration;
    public List<float> CheckPoints = new List<float>();
}
