using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] protected ScriptableObject info;
    [SerializeField] private GameObject collectible;
    public bool Collected = false;

    /// <summary>
    /// Chama o efeito após coletar e passa infos para outros códigos.
    /// Chamar código original no fim do override.
    /// </summary>
    protected virtual void Collect()
    {
        Collected = true;
        CollectEffect();
        CollectInfo();
    }

    protected virtual void CollectEffect()
    {
        collectible.SetActive(false);
    }

    /// <summary>
    /// Passa para outros códigos a informação de que o coletavel foi pego.
    /// </summary>
    protected virtual void CollectInfo()
    {
        
    }
}
