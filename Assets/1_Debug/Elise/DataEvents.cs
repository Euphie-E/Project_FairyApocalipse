using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System;
using UnityEngine.UnityConsent;
using Unity.Services.Core.Environments;


public class DataEvents
{
    public bool isInitialized = false;
    public enum Statistics {
        JumpTotal,
        Jump_1,
        Jump_2,
        Jump_3,
        Jump_4,
        TimeTravelTotal,
        TimeTravel_1,
        TimeTravel_2,
        TimeTravel_3,
        TimeTravel_4,
        DeathTotal,
        Death_1,
        Death_2,
        Death_3,
        Death_4
    };
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
    }
    public void Add(Statistics statistics,int value)
    {
        if(!isInitialized) return;
        CustomEvent myEvent = new CustomEvent(statistics.ToString())
        {
            {statistics.ToString(), value},
            {"UserID",SystemInfo.deviceUniqueIdentifier}
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
    }
    
}

public class PlayerData
{
    public int JumpTotal;
    public int Jump_1;
    public int Jump_2;
    public int Jump_3;
    public int Jump_4;
    public int TimeTravelTotal;
    public int TimeTravel_1;
    public int TimeTravel_2;
    public int TimeTravel_3;
    public int TimeTravel_4;
    public int DeathTotal;
    public int Death_1;
    public int Death_2;
    public int Death_3;
    public int Death_4;
}
