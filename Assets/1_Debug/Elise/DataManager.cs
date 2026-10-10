using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance {get; private set;}
    public readonly DataEvents dataEvents = new();
    string savePath;
    PlayerDataGroup playerDataGroup = new(5);
    PlayerData playerData;
    [SerializeField] Environment ENVIRONMENT = Environment.production;
    [SerializeField] SceneAsset[] scenes = new SceneAsset[4]; 
    float strTime;
    int ckpSize = 4;
    private bool testing = false;
    InputAction add;
    InputSystem_Actions act;
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
        act = new InputSystem_Actions();
        add = act.Player.AddTester;
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
        if(ENVIRONMENT == Environment.playtest) testing = true;
        SceneManager.activeSceneChanged += SaveGame;
        SceneManager.sceneLoaded += Run;
        add.performed += AddNewTester;
    }
    void OnEnable()
    {
        add.Enable();
    }
    void OnDisable()
    {
        add.Disable();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    void SaveData()
    {
        playerData.Duration = (int)((Time.time - strTime)*100)/100f;
        string json = JsonUtility.ToJson(playerDataGroup, true);
        File.WriteAllText(savePath, json);
    }
    public void SendData()
    {
        if (TestInit())
        {
            dataEvents.Add(DataEvents.Statistics.JumpTotal,playerDataGroup.JumpTotal);
            dataEvents.Add(DataEvents.Statistics.Jump,playerData.Jump);
            dataEvents.Add(DataEvents.Statistics.DeathTotal,playerDataGroup.DeathTotal);
            dataEvents.Add(DataEvents.Statistics.Death,playerData.Death);
            dataEvents.Add(DataEvents.Statistics.TimeTravelTotal,playerDataGroup.TimeTravelTotal);
            dataEvents.Add(DataEvents.Statistics.TimeTravel,playerData.TimeTravel);
            dataEvents.Add(DataEvents.Statistics.GDuration,(int)playerData.Duration);
            if(playerData.CheckPoints.Length > 0)
            {
                dataEvents.Add(DataEvents.Statistics.CheckPoint,(float)playerData.CheckPoints[0]-strTime,0);
                for (int i = 1; i < playerData.CheckPoints.Length; i++)
                {
                    if(playerData.CheckPoints[i] == 0) return;
                    dataEvents.Add(DataEvents.Statistics.CheckPoint,(float)(playerData.CheckPoints[i]-playerData.CheckPoints[i-1]),i);
                }
            }
        } 
    }
    void Run(Scene scene, LoadSceneMode loaded)
    {
        if(playerDataGroup == null) LoadGame();
        int index = CheckSceneIDX(scene);
        dataEvents.index = index;
        if(testing || playerData == null || index != playerData.index)
        {
            playerData = new();
            strTime = Time.time;
        }
        playerDataGroup.group[index] = playerData;
        playerData.index = index;
        if(playerData.CheckPoints == null || playerData.CheckPoints.Length != ckpSize) playerData.CheckPoints = new double[ckpSize];
    }
    void SaveGame(Scene current, Scene Next)
    {
        if (testing) return;
        SaveData();
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
        playerDataGroup ??= new PlayerDataGroup(5);
        if(playerDataGroup.group == null || playerDataGroup.group.Length == 0) playerDataGroup = new PlayerDataGroup(5);
    }
    
    public void AddData(Data data, int value)
    {
        if(!TestInit()) return;
        switch (data)
        {
            case Data.Death:
                playerData.Death += value;
                playerDataGroup.DeathTotal += value;
                break;
            case Data.Jump:
                playerData.Jump += value;
                playerDataGroup.JumpTotal += value;
                break;
            case Data.TimeTravel:
                playerData.TimeTravel += value;
                playerDataGroup.TimeTravelTotal += value;
                break;
            default:
                break;
        }
        
    }
    int ckpPt = 0;
    public void AddData(Data data, float value)
    {
        if(!TestInit()) return;
        switch (data)
        {
            case Data.TimeStamp:
                playerData.CheckPoints[ckpPt] = (int)(value*100)/100f;
                break;
            default:
                break;
        }
    }
    bool TestInit()
    {
        return dataEvents.isInitialized;
    }
    public void SetTotalCheckpoints(int size)
    {
        ckpSize = size;
        if(playerData != null)
        {
            if(playerData.CheckPoints == null || playerData.CheckPoints.Length != ckpSize)
                playerData.CheckPoints = new double[ckpSize];
        }
    }
    void AddNewTester(InputAction.CallbackContext ctx = new())
    {
        testing = true;
        int user = dataEvents.AddPlayerCT(PlayerPrefs.GetInt("TotalTester",0));
        if(playerData != null && playerData.index != -1)
        {
            playerData.Duration = (int)((Time.time - strTime)*100)/100f;
            string json = JsonUtility.ToJson(playerDataGroup, true);
            string folderPath = Path.Combine(Application.persistentDataPath, "TestData");
            string fName = "playerData_"+user.ToString()+".json";
            string filePath = Path.Combine(folderPath, fName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            } 
            File.WriteAllText(filePath, json);
            SendData(); 
        }
        PlayerPrefs.SetInt("TotalTester", user);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
