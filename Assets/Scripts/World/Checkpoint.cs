using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index = 0;
    [SerializeField] private Animator animatorCheckpoint;
    [SerializeField] private VisualEffect vfxCheckpoint;

    void Start()
    {
        PlayerInput.Instance?.AddCheckpoint(gameObject, index);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeathController playerDeath = other.GetComponent<PlayerDeathController>();

            if (playerDeath != null)
            {
                playerDeath.NewCheckpoint(index, gameObject);
            }
        }
    }

    public void PlayRespawnEffect()
    {
        if (animatorCheckpoint != null)
        {
            animatorCheckpoint.SetTrigger("Respawn");
        }

        if (vfxCheckpoint != null)
        {
            vfxCheckpoint.gameObject.SetActive(true);
            vfxCheckpoint.Stop();
            vfxCheckpoint.Play();
        }
    }
}
