using UnityEngine;

public class GnomeHUDRefreshOnOpen : MonoBehaviour
{
    [SerializeField] private GnomeHUD hud;

    private void OnEnable()
    {
        if (hud != null)
        {
            hud.RefreshHUD();
        }
    }
}