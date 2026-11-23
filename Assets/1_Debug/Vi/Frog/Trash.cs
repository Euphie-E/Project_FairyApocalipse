using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] TrashManager manager;
    [SerializeField] int life = 3;
    [SerializeField] GameObject[] pileStates;

    public bool IsAlive => life > 0;

    public void TakeDamage()
    {
        if (life <= 0) return;

        life--;

        if (pileStates != null &&
            life >= 0 &&
            life < pileStates.Length)
        {
            GameObject state = pileStates[life];

            if (state != null)
            {
                // Verifica se o estado possui efeitos
                TrashStateEffects effects =
                    state.GetComponent<TrashStateEffects>();

                // Executa os efeitos, caso existam
                if (effects != null)
                {
                    effects.PlayEffects();
                }

                // Desativa o estado destruído
                state.SetActive(false);
            }
        }

        if (life == 0)
        {
            if (manager != null)
                manager.DestroyPile();
        }
    }
}