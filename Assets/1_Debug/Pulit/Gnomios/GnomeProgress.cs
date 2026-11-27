
using System;
using UnityEngine;

public static class GnomeProgress
{
    public const int TotalGnomes = 14;

    public static event Action OnProgressChanged;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        OnProgressChanged = null;
    }

    public static bool IsCollected(int id)
    {
        if (DataManager.Instance == null)
            return false;

        return DataManager.Instance.IsGnomeCollected(id);
    }

    public static bool Collect(int id)
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning(
                "[GNOMOS] DataManager nao encontrado!"
            );
            return false;
        }

        bool collected =
            DataManager.Instance.CollectGnome(id);

        if (!collected)
            return false;

        NotifyChanged();

        Debug.Log(
            $"[GNOMOS] Total: {GetTotalCollected()}/14"
        );

        return true;
    }

    public static int GetTotalCollected()
    {
        int count = 0;

        for (int id = 1; id <= TotalGnomes; id++)
        {
            if (IsCollected(id))
                count++;
        }

        return count;
    }

    public static int GetPhaseCollected(int phase)
    {
        if (phase < 1 || phase > 2)
            return 0;

        int count = 0;
        int startId = (phase - 1) * 7 + 1;

        for (int id = startId; id < startId + 7; id++)
        {
            if (IsCollected(id))
                count++;
        }

        return count;
    }

    public static void NotifyChanged()
    {
        OnProgressChanged?.Invoke();
    }

    public static void ResetAll()
    {
        if (DataManager.Instance == null)
            return;

        DataManager.Instance.ResetGnomes();

        NotifyChanged();
    }
}
