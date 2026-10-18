
using UnityEngine;

public class BossAttackWarningEvents : MonoBehaviour
{
    [Header("Caudas")]
    public BossAttackWarning caudaEsquerda;
    public BossAttackWarning caudaMeio;
    public BossAttackWarning caudaDireita;

    [Header("Pisada")]
    public BossAttackWarning[] patas;

    public void CaudaEsquerdaAvisoOn()
    {
        if (caudaEsquerda != null)
            caudaEsquerda.Show();
    }

    public void CaudaEsquerdaAvisoOff()
    {
        if (caudaEsquerda != null)
            caudaEsquerda.Hide();
    }

    public void CaudaMeioAvisoOn()
    {
        if (caudaMeio != null)
            caudaMeio.Show();
    }

    public void CaudaMeioAvisoOff()
    {
        if (caudaMeio != null)
            caudaMeio.Hide();
    }

    public void CaudaDireitaAvisoOn()
    {
        if (caudaDireita != null)
            caudaDireita.Show();
    }

    public void CaudaDireitaAvisoOff()
    {
        if (caudaDireita != null)
            caudaDireita.Hide();
    }

    public void PisadaAvisoOn()
    {
        foreach (BossAttackWarning aviso in patas)
        {
            if (aviso != null)
                aviso.Show();
        }
    }

    public void PisadaAvisoOff()
    {
        foreach (BossAttackWarning aviso in patas)
        {
            if (aviso != null)
                aviso.Hide();
        }
    }

    public void HideAll()
    {
        CaudaEsquerdaAvisoOff();
        CaudaMeioAvisoOff();
        CaudaDireitaAvisoOff();
        PisadaAvisoOff();
    }

    private void OnDisable()
    {
        HideAll();
    }
}
