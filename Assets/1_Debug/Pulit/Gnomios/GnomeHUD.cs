
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GnomeHUD : MonoBehaviour
{
    [Serializable]
    public class GnomeSlot
    {
        [Range(1, 14)]
        public int gnomeId = 1;

        public Image image;
        public Sprite collectedSprite;
    }

    [Header("Silhueta padrao")]
    [SerializeField]
    private Sprite silhouetteSprite;

    [Header("Lista de gnomios")]
    [SerializeField]
    private List<GnomeSlot> slots = new();

    private void OnEnable()
    {
        GnomeProgress.OnProgressChanged += RefreshHUD;

        RefreshHUD();
    }

    private void Start()
    {
        RefreshHUD();
    }

    private void OnDisable()
    {
        GnomeProgress.OnProgressChanged -= RefreshHUD;
    }

    [ContextMenu("Atualizar HUD")]
    public void RefreshHUD()
    {
        foreach (GnomeSlot slot in slots)
        {
            if (slot.image == null)
            {
                Debug.LogWarning(
                    $"[GNOME HUD] Image nao configurada no ID {slot.gnomeId}!",
                    this
                );
                continue;
            }

            bool collected =
                GnomeProgress.IsCollected(slot.gnomeId);

            Sprite selectedSprite = collected
                ? slot.collectedSprite
                : silhouetteSprite;

            if (selectedSprite == null)
            {
                Debug.LogWarning(
                    $"[GNOME HUD] Sprite ausente no ID {slot.gnomeId}!",
                    this
                );
                continue;
            }

            slot.image.sprite = selectedSprite;
            slot.image.color = Color.white;

            // Atualiza o componente visual.
            slot.image.SetAllDirty();

            Debug.Log(
                $"[GNOME HUD] ID {slot.gnomeId} | " +
                $"Coletado: {collected} | " +
                $"Sprite: {selectedSprite.name}"
            );
        }
    }
}
