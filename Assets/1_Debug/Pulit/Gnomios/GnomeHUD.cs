
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
        public int gnomeId;

        [Header("Imagem na HUD")]
        public Image image;

        [Header("Sprite do gnomo coletado")]
        public Sprite collectedSprite;
    }

    [Header("Silhueta padrão")]
    [SerializeField]
    private Sprite silhouetteSprite;

    [Header("Lista de Gnomos")]
    [SerializeField]
    private List<GnomeSlot> slots = new();

    private void OnEnable()
    {
        GnomeProgress.OnProgressChanged += RefreshHUD;
        RefreshHUD();
    }

    private void OnDisable()
    {
        GnomeProgress.OnProgressChanged -= RefreshHUD;
    }

    public void RefreshHUD()
    {
        foreach (GnomeSlot slot in slots)
        {
            if (slot.image == null)
                continue;

            bool collected =
                GnomeProgress.IsCollected(slot.gnomeId);

            slot.image.sprite = collected
                ? slot.collectedSprite
                : silhouetteSprite;

            slot.image.color = Color.white;
        }
    }
}
