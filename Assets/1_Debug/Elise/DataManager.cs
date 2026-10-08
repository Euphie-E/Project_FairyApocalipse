using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public bool b;
    void Update()
    {
        if (b)
        {
            Debug.Log(SceneManager.GetActiveScene().name == scenes[0].name);
            b=!b;
        }
    }
    public static DataManager Instance {get; private set;}
    public readonly DataEvents dataEvents = new();
    string savePath;
    PlayerDataGroup playerDataGroup = new(5);
    PlayerData playerData;
    [SerializeField] Environment ENVIRONMENT = Environment.production;
    [SerializeField] SceneAsset[] scenes = new SceneAsset[4]; 
    float strTime;
    public enum Environment
    {
        production,
        playtest
    }
    public enum Data
    {
        Jump,
        Death,
        TimeTravel,
        TimeStamp
    }
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            dataEvents.Run(ENVIRONMENT.ToString());
        }
    }
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        LoadGame();
        Run(SceneManager.GetActiveScene(),LoadSceneMode.Single);
        SceneManager.activeSceneChanged += SaveGame;
        SceneManager.sceneLoaded += Run;
    }

    void OnApplicationQuit()
    {
        SendData();
    }

    public void SendData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath, json);
        if (dataEvents.isInitialized)
        {
            dataEvents.Add(DataEvents.Statistics.JumpTotal,PlayerData.JumpTotal);
            dataEvents.Add(DataEvents.Statistics.Jump,playerData.Jump);
            dataEvents.Add(DataEvents.Statistics.DeathTotal,PlayerData.DeathTotal);
            dataEvents.Add(DataEvents.Statistics.Death,playerData.Death);
            dataEvents.Add(DataEvents.Statistics.TimeTravelTotal,PlayerData.TimeTravelTotal);
            dataEvents.Add(DataEvents.Statistics.TimeTravel,playerData.TimeTravel);
            dataEvents.Add(DataEvents.Statistics.GDuration,playerData.Duration);
            if(playerData.CheckPoints.Count > 0)
            {
                dataEvents.Add(DataEvents.Statistics.CheckPoints,playerData.CheckPoints[0]-strTime,0);
                for (int i = 1; i < playerData.CheckPoints.Count; i++)
                {
                    dataEvents.Add(DataEvents.Statistics.CheckPoints,playerData.CheckPoints[i]-playerData.CheckPoints[i-1],i);
                }
            }
        } 
    }
    void Run(Scene scene, LoadSceneMode loaded)
    {
        int index = CheckSceneIDX(scene);
        dataEvents.index = index;
        playerData = playerDataGroup.group[index];
        playerData.index = index;
        strTime = Time.time;
    }
    void SaveGame(Scene current, Scene Next)
    {
        playerData.Duration = Time.time - strTime;
        string json = JsonUtility.ToJson(playerDataGroup, true);
        File.WriteAllText(savePath, json);
        SendData();
    }

    int CheckSceneIDX(Scene scene)
    {
        int index = 4;
        if(scene.name == scenes[0].name)
        {
            index = 0;
        }
        else if(scene.name == scenes[1].name)
        {
            index = 1;
        }
        else if(scene.name == scenes[2].name)
        {
            index = 2;
        }
        else if(scene.name == scenes[3].name)
        {
            index = 3;
        }
        else
        {
            Debug.Log("this scene is not loaded in the data manager at: "+this.gameObject.name);
        }
        return index;
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            playerDataGroup =  new PlayerDataGroup(5); 
            return;
        }

        // 2. Read the raw text from the file
        string json = File.ReadAllText(savePath);

        // 3. Convert the JSON string back into your C# object type
        PlayerDataGroup data = JsonUtility.FromJson<PlayerDataGroup>(json);

        playerDataGroup = data;
    }
    
    public void AddData(Data data, int value)
    {
        switch (data)
        {
            case Data.Death:
                playerData.Death += value;
                PlayerData.DeathTotal += value;
                break;
            case Data.Jump:
                playerData.Jump += value;
                PlayerData.JumpTotal += value;
                break;
            case Data.TimeTravel:
                playerData.TimeTravel += value;
                PlayerData.TimeTravelTotal += value;
                break;
            default:
                break;
        }
        
    }
    public void AddData(Data data, float value)
    {
        switch (data)
        {
            case Data.TimeStamp:
                playerData.CheckPoints.Append(value);
                break;
            default:
                break;
        }
    }
}
