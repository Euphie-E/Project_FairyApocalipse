
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : Savable
{
    private static readonly int RespawnHash =
        Animator.StringToHash("Respawn");

    [Header("Checkpoint")]
    [SerializeField] public int index = 0;

    [Header("Reativacao")]
    [Tooltip("Permite ativar novamente este checkpoint.")]
    [SerializeField] private bool allowReactivation = false;

    private bool hasBeenActivated = false;

    [Header("VFX")]
    [SerializeField] private Animator animatorCheckpoint;
    [SerializeField] private VisualEffect vfxCheckpoint;

    void Start()
    {
        saveData = new Save(Types.Checkpoint, index);

        // -1 significa que nunca foi ativado.
        saveData.Add(new KeyValue
        {
            key = "Count",
            value = -1
        });

        LevelManager.Get()?.Add(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Impede reativacao quando desabilitada.
        if (hasBeenActivated && !allowReactivation)
            return;

        LevelManager.Get()?.SetLastCheckpoint(index);
    }

    // Chamado pelo LevelManager quando ativado.
    public void RegisterActivation(int order)
    {
        hasBeenActivated = true;

        // Recria os dados para nao duplicar Count.
        saveData = new Save(Types.Checkpoint, index);

        saveData.Add(new KeyValue
        {
            key = "Count",
            value = order
        });

        Debug.Log(
            $"Checkpoint {index} ativado! Ordem: {order}"
        );
    }

    public void PlayRespawnEffect()
    {
        if (animatorCheckpoint != null)
        {
            animatorCheckpoint.SetTrigger(RespawnHash);
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
        if (save == null)
            return;

        Save saved = save
            .Where(s =>
                s.Key == Types.Checkpoint &&
                s.ID == index)
            .OrderByDescending(s => s.Get("Count"))
            .FirstOrDefault();

        if (saved == null)
            return;

        int count = saved.Get("Count");

        hasBeenActivated = count >= 0;
        saveData = saved;
    }
}
