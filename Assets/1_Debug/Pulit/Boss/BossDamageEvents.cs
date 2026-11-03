
using UnityEngine;

public class BossDamageEvents : MonoBehaviour
{
    [Header("Caudas")]
    public BossDamageTrigger caudaEsquerda;
    public BossDamageTrigger caudaMeio;
    public BossDamageTrigger caudaDireita;

    [Header("Patas")]
    public BossDamageTrigger[] patas;

    private void Awake()
    {
        DisableAll();
    }

    // CAUDA ESQUERDA
    public void CaudaEsquerdaOn()
    {
        if (caudaEsquerda != null)
            caudaEsquerda.SetDamageActive(true);
    }

    public void CaudaEsquerdaOff()
    {
        if (caudaEsquerda != null)
            caudaEsquerda.SetDamageActive(false);
    }

    // CAUDA DO MEIO
    public void CaudaMeioOn()
    {
        if (caudaMeio != null)
            caudaMeio.SetDamageActive(true);
    }

    public void CaudaMeioOff()
    {
        if (caudaMeio != null)
            caudaMeio.SetDamageActive(false);
    }

    // CAUDA DIREITA
    public void CaudaDireitaOn()
    {
        if (caudaDireita != null)
            caudaDireita.SetDamageActive(true);
    }

    public void CaudaDireitaOff()
    {
        if (caudaDireita != null)
            caudaDireita.SetDamageActive(false);
    }

    // PISADA
    public void PatasOn()
    {
        foreach (BossDamageTrigger pata in patas)
        {
            if (pata != null)
                pata.SetDamageActive(true);
        }
    }

    public void PatasOff()
    {
        foreach (BossDamageTrigger pata in patas)
        {
            if (pata != null)
                pata.SetDamageActive(false);
        }
    }

    // SEGURANCA
    public void DisableAll()
    {
        CaudaEsquerdaOff();
        CaudaMeioOff();
        CaudaDireitaOff();
        PatasOff();
    }

    private void OnDisable()
    {
        DisableAll();
    }
}
