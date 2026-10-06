using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance {get; private set;}
    DataEvents dataEvents = new DataEvents();
    string savePath;
    PlayerData playerData;
    [SerializeField] string ENVIRONMENT = "production";
    [SerializeField] Scene[] scenes = new Scene[4]; 
    public enum Data
    {
        Jump,
        Death,
        TimeTravel
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
            dataEvents.Run(ENVIRONMENT);
        }
    }
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        LoadGame();
        SceneManager.activeSceneChanged += SaveGame;
    }

    void OnApplicationQuit()
    {
        SaveGame();
        
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath, json);
        if (dataEvents.isInitialized)
        {
            dataEvents.Add(DataEvents.Statistics.JumpTotal,playerData.JumpTotal);
            dataEvents.Add(DataEvents.Statistics.Jump_1,playerData.Jump_1);
            dataEvents.Add(DataEvents.Statistics.Jump_2,playerData.Jump_2);
            dataEvents.Add(DataEvents.Statistics.Jump_3,playerData.Jump_3);
            dataEvents.Add(DataEvents.Statistics.Jump_4,playerData.Jump_4);
            dataEvents.Add(DataEvents.Statistics.DeathTotal,playerData.DeathTotal);
            dataEvents.Add(DataEvents.Statistics.Death_1,playerData.Death_1);
            dataEvents.Add(DataEvents.Statistics.Death_2,playerData.Death_2);
            dataEvents.Add(DataEvents.Statistics.Death_3,playerData.Death_3);
            dataEvents.Add(DataEvents.Statistics.Death_4,playerData.Death_4);
            dataEvents.Add(DataEvents.Statistics.TimeTravelTotal,playerData.TimeTravelTotal);
            dataEvents.Add(DataEvents.Statistics.TimeTravel_1,playerData.TimeTravel_1);
            dataEvents.Add(DataEvents.Statistics.TimeTravel_2,playerData.TimeTravel_2);
            dataEvents.Add(DataEvents.Statistics.TimeTravel_3,playerData.TimeTravel_3);
            dataEvents.Add(DataEvents.Statistics.TimeTravel_4,playerData.TimeTravel_4);
        }
    }

    void SaveGame(Scene current, Scene Next)
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            playerData =  new PlayerData(); 
        }

        // 2. Read the raw text from the file
        string json = File.ReadAllText(savePath);

        // 3. Convert the JSON string back into your C# object type
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        playerData = data;
    }
    
    public void AddData(Data data, int value)
    {
        Scene scene = SceneManager.GetActiveScene();
        switch (data)
        {
            case Data.Jump:
                playerData.JumpTotal += value;
                if(scene == scenes[0])
                {
                    playerData.Jump_1 += value;
                }
                else if(scene == scenes[1])
                {
                    playerData.Jump_2 += value;
                }
                else if(scene == scenes[2])
                {
                    playerData.Jump_3 += value;
                }
                else if(scene == scenes[3])
                {
                    playerData.Jump_4 += value;
                }
                else
                {
                    Debug.Log("this scene is not loaded in the data manager at: "+this.gameObject.name);
                }
                break;
            case Data.Death:
                playerData.DeathTotal += value;
                if(scene == scenes[0])
                {
                    playerData.Death_1 += value;
                }
                else if(scene == scenes[1])
                {
                    playerData.Death_2 += value;
                }
                else if(scene == scenes[2])
                {
                    playerData.Death_3 += value;
                }
                else if(scene == scenes[3])
                {
                    playerData.Death_4 += value;
                }
                else
                {
                    Debug.Log("this scene is not loaded in the data manager at: "+this.gameObject.name);
                }
                break;
            case Data.TimeTravel:
                playerData.TimeTravelTotal += value;
                if(scene == scenes[0])
                {
                    playerData.TimeTravel_1 += value;
                }
                else if(scene == scenes[1])
                {
                    playerData.TimeTravel_2 += value;
                }
                else if(scene == scenes[2])
                {
                    playerData.TimeTravel_3 += value;
                }
                else if(scene == scenes[3])
                {
                    playerData.TimeTravel_4 += value;
                }
                else
                {
                    Debug.Log("this scene is not loaded in the data manager at: "+this.gameObject.name);
                }

                break;
            default:
                Debug.Log("wrong enum for data at: "+this.gameObject.name);
                break;
        }
    }
}
