using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : Savable
{
    [Header("VFX")]
    [SerializeField] private int index = 0;
    [SerializeField] private Animator animatorCheckpoint;
    [SerializeField] private VisualEffect vfxCheckpoint;
    static int ct = 0;

    void Start()
    {
        PlayerInput.Instance?.AddCheckpoint(gameObject, index);
        saveData = new Save(Types.Checkpoint,index);
        LevelManager.Instance?.Add(this);
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
            saveData.Add(new KeyValue{key = "Count",value =ct++});
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

    public override Save Save()
    {
        return saveData;
    }

    public override void Load(List<Save> save)
    {
        
    }

}
