using UnityEngine;

public class CampfireController : MonoBehaviour
{
    [SerializeField] private float raioInteracaoFogueira = 3f;


    public void InteragirComFogueira()
    {
        Collider[] encontrados = Physics.OverlapSphere(
            transform.position,
            raioInteracaoFogueira
        );

        FogueiraVFX fogueiraProxima = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider col in encontrados)
        {
            FogueiraVFX fogueira =
                col.GetComponentInParent<FogueiraVFX>();

            if (fogueira == null) continue;

            float distancia = Vector3.Distance(
                transform.position,
                fogueira.transform.position
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                fogueiraProxima = fogueira;
            }
        }

        if (fogueiraProxima != null)
        {
            fogueiraProxima.AlternarFogueira();
        }
    }
}
