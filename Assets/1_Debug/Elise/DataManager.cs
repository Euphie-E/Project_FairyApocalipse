using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds180 = new WaitForSeconds(30);
    [Header("Gnomos")]
    private const int TotalGnomes = 14;
    public static DataManager Instance {get; private set;}
    public readonly DataEvents dataEvents = new();
    string savePath;
    PlayerDataGroup playerDataGroup = new(5);
    PlayerData playerData;
    [SerializeField] Environment ENVIRONMENT = Environment.production;
    [SerializeField] String[] scenes = new String[4]; 
    float strTime;
    int ckpSize = 4;
    private bool testing = false;
    InputAction add;
    InputSystem_Actions act;
    bool reset = false;
    public enum Environment
    {
        disable,
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

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "playerData.json"
        );

        testing = ENVIRONMENT == Environment.playtest;

        // Carrega o save uma unica vez.
        LoadGame();

        EnsureGnomeData();

        // No Editor ou em playtest:
        // nova execucao = nova colecao.
        if (Application.isEditor || testing)
        {
            playerDataGroup.CollectedGnomes = new bool[14];

        }

        dataEvents.Run(ENVIRONMENT.ToString());
    }


    void Start()
    {
        Run(
            SceneManager.GetActiveScene(),
            LoadSceneMode.Single
        );

        SceneManager.activeSceneChanged += SaveGame;
        SceneManager.sceneLoaded += Run;

        add.performed += AddNewTester;

        StartCoroutine(Cycle());

        GnomeProgress.NotifyChanged();
    }

    IEnumerator Cycle()
    {
        yield return new WaitForFixedUpdate();
        LevelManager.Get()?.Load(Get());
        while (true)
        {
            yield return _waitForSeconds180;
            playerData.LevelData = LevelManager.Get()?.Save();
        }
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
        if (playerData != null)
        {
            playerData.Duration =
                (int)((Time.time - strTime) * 100) / 100f;

            LevelManager level = LevelManager.Get();

            if (level != null)
            {
                playerData.LevelData = level.Save();
            }
        }

        string json = JsonUtility.ToJson(
            playerDataGroup,
            true
        );

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

        int index = CheckSceneIDX(scene);
        dataEvents.index = index;

        if (testing || reset ||
            playerDataGroup.group[index] == null)
        {
            playerData = new PlayerData();
            strTime = Time.time;

            playerDataGroup.group[index] = playerData;
        }
        else
        {
            playerData = playerDataGroup.group[index];
        }

        playerData.index = index;

        if (playerData.CheckPoints == null ||
            playerData.CheckPoints.Length != ckpSize)
        {
            playerData.CheckPoints = new double[ckpSize];
        }

        Debug.Log(
            $"[DATA MANAGER] Cena: {scene.name} | " +
            $"Gnomos coletados: {GnomeProgress.GetTotalCollected()}/14"
        );
        //if(reset) return;
        //if(!(index >= 0 && index <= 3)) return;
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<PlayerPocket>().seed = playerData.seed;
        //double pos = -1;
        //for (int i = 0; i<playerData.CheckPoints.Length; i++)
        //{
        //    
        //}
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
        if(scene.name == scenes[0])
        {
            index = 0;
        }
        else if(scene.name == scenes[1])
        {
            index = 1;
        }
        else if(scene.name == scenes[2])
        {
            index = 2;
        }
        else if(scene.name == scenes[3])
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
    public void AddData(Data data, float value, int pos)
    {
        switch (data)
        {
            case Data.TimeStamp:
                if(pos >= ckpSize) return;
                playerData.CheckPoints[pos] = (int)(value*100)/100f;
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
        GnomeProgress.ResetAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public List<Save> Get()
    {
        return playerData.LevelData;
    }

    private void EnsureGnomeData()
    {
        if (playerDataGroup == null)
            playerDataGroup = new PlayerDataGroup(5);

        if (playerDataGroup.CollectedGnomes == null ||
            playerDataGroup.CollectedGnomes.Length != TotalGnomes)
        {
            bool[] old = playerDataGroup.CollectedGnomes;

            playerDataGroup.CollectedGnomes = new bool[TotalGnomes];

            if (old != null)
            {
                Array.Copy(
                    old,
                    playerDataGroup.CollectedGnomes,
                    Mathf.Min(old.Length, TotalGnomes)
                );
            }
        }
    }

    public bool IsGnomeCollected(int id)
    {
        if (id < 1 || id > TotalGnomes)
            return false;

        EnsureGnomeData();

        return playerDataGroup.CollectedGnomes[id - 1];
    }

    public bool CollectGnome(int id)
    {
        if (id < 1 || id > TotalGnomes)
            return false;

        EnsureGnomeData();

        if (playerDataGroup.CollectedGnomes[id - 1])
            return false;

        playerDataGroup.CollectedGnomes[id - 1] = true;

        Debug.Log($"[GNOMOS] Gnomo {id} coletado!");

        // Salva imediatamente na partida normal.
        if (!testing)
            SaveData();

        return true;
    }

    public void ResetGnomes()
    {
        EnsureGnomeData();

        playerDataGroup.CollectedGnomes = new bool[TotalGnomes];

        if (!testing)
            SaveData();

        Debug.Log("[GNOMOS] Colecao resetada!");
    }
}
