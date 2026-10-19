using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPlantStep : MonoBehaviour
{
    private CharacterController controller;
    private PlantWobble plataformaAtual;
    private float tempoNoAr;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Libera a plataforma anterior depois de saltar
        if (!controller.isGrounded)
            tempoNoAr += Time.deltaTime;
        else
            tempoNoAr = 0f;

        if (tempoNoAr > 0.15f)
            plataformaAtual = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PlantWobble planta =
            hit.collider.GetComponentInParent<PlantWobble>();

        if (planta == null)
            return;

        if (hit.normal.y < 0.55f)
            return;

        if (planta == plataformaAtual)
            return;

        plataformaAtual = planta;

        planta.Balancar();
    }
}
