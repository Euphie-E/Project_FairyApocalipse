using UnityEngine;

[CreateAssetMenu(fileName = "Seed", menuName = "Key/Seed")]
public class Seed : ScriptableObject
{
    public int seedIndex;
    public GameObject plantedPrefab;
}
