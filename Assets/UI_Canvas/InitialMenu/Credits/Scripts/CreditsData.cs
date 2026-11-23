using UnityEngine;

[CreateAssetMenu(fileName = "CreditsData", menuName = "Scriptable Objects/CreditsData")]
public class CreditsData : ScriptableObject
{
    public Sprite image, logo;
    [TextArea] public string texto;
    public string name;
}
