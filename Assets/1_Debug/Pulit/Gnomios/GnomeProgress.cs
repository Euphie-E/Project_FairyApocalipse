
using System;
using UnityEngine;

public static class GnomeProgress
{
    public const int TotalGnomes = 14;

    private const string SaveKey = "Ecos_Gnomes";

    [Serializable]
    private class SaveData
    {
        public bool[] collected = new bool[TotalGnomes];
    }

    private static SaveData data;

    public static event Action OnProgressChanged;

    private static void Load()
    {
        if (data != null) return;

        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
        {
            data = new SaveData();
            return;
        }

        try
        {
            data = JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            data = new SaveData();
        }

        if (data == null)
            data = new SaveData();

        if (data.collected == null ||
            data.collected.Length != TotalGnomes)
        {
            bool[] previous = data.collected;
            data.collected = new bool[TotalGnomes];

            if (previous != null)
                Array.Copy(
                    previous,
                    data.collected,
                    Mathf.Min(previous.Length, TotalGnomes)
                );
        }
    }

    public static bool IsCollected(int id)
    {
        Load();

        if (id < 1 || id > TotalGnomes)
            return false;

        return data.collected[id - 1];
    }

    public static bool Collect(int id)
    {
        Load();

        if (id < 1 || id > TotalGnomes)
            return false;

        if (data.collected[id - 1])
            return false;

        data.collected[id - 1] = true;

        Save();

        OnProgressChanged?.Invoke();

        Debug.Log($"Gnomo {id} coletado! " +
                  $"Total: {GetTotalCollected()}/14");

        return true;
    }

    public static int GetTotalCollected()
    {
        Load();

        int count = 0;

        foreach (bool collected in data.collected)
        {
            if (collected)
                count++;
        }

        return count;
    }

    public static int GetPhaseCollected(int phase)
    {
        Load();

        if (phase < 1 || phase > 2)
            return 0;

        int start = (phase - 1) * 7;
        int count = 0;

        for (int i = start; i < start + 7; i++)
        {
            if (data.collected[i])
                count++;
        }

        return count;
    }

    private static void Save()
    {
        PlayerPrefs.SetString(
            SaveKey,
            JsonUtility.ToJson(data)
        );

        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        data = new SaveData();

        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        OnProgressChanged?.Invoke();
    }
}
