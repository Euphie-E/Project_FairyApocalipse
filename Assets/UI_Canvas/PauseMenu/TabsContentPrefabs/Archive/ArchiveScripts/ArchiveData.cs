using UnityEngine;

[CreateAssetMenu(fileName = "ArchiveData", menuName = "Scriptable Objects/ArchiveData")]
public class ArchiveData : ScriptableObject
{
    public string tName;
    [TextArea] public string description;
    public Sprite icon;
}
