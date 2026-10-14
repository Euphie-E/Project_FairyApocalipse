using UnityEngine;

public class Collectible : MonoBehaviour
{
    private GameObject collectible;
    public bool Collected = false;

    void Start()
    {
        collectible = transform.GetChild(0).gameObject;
    }

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
