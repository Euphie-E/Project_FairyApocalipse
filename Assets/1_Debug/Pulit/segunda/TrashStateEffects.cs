using UnityEngine;

public class TrashStateEffects : MonoBehaviour
{
    [Header("VFX na Cena (Opcional)")]
    [SerializeField] private TrashExplosionVFX vfx;

    [Header("SFX (Opcional)")]
    [SerializeField] private AudioClip sfx;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    public void PlayEffects()
    {
        // VFX
        if (vfx != null)
        {
            vfx.Executar();
        }

        // SFX
        if (sfx != null)
        {
            AudioSource.PlayClipAtPoint(
                sfx,
                transform.position,
                volume
            );
        }
    }
}