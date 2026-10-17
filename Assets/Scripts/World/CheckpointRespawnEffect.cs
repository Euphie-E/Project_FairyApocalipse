
using UnityEngine;
using UnityEngine.VFX;

public class CheckpointRespawnEffect : MonoBehaviour
{
    [SerializeField] private Animator animatorcheckpoint;
    [SerializeField] private VisualEffect vfxcheckpoint;

    public void PlayRespawnEffect()
    {
        if (animatorcheckpoint != null)
        {
            animatorcheckpoint.SetTrigger("Respawn");
        }

        if (vfxcheckpoint != null)
        {
            vfxcheckpoint.Stop();
            vfxcheckpoint.Play();
            Destroy(vfxcheckpoint, 2f);
        }
    }
}
