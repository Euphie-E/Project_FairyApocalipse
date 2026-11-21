using UnityEngine;

[CreateAssetMenu(fileName = "NotifData", menuName = "Scriptable Objects/NotifData")]
public class NotifData : ScriptableObject
{
    [TextArea]
    public string notifText;
    public float duration = 2f;
    public float fade = 1f;   
}
