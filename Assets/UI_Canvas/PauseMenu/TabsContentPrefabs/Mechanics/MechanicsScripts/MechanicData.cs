using UnityEngine;

[CreateAssetMenu(fileName = "MechanicData", menuName = "Scriptable Objects/MechanicData")]
public class MechanicData : ScriptableObject
{
    public string tName, keyKeyboard;
    public string description;
    public Sprite[] gif;
    public Sprite keyControl;
}
