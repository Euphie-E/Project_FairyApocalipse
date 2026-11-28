using UnityEngine;

// Mantem a camera habilitada durante a atualizacao dos objetivos (1100),
// e bloqueia a renderizacao nos frames dispensados pelo controller (1000).
[DefaultExecutionOrder(1200)]
[DisallowMultipleComponent]
[AddComponentMenu("")]
public sealed class MinimapRenderGate : MonoBehaviour
{
    private MinimapController controller;

    public void DefinirController(MinimapController novoController)
    {
        controller = novoController;
    }

    private void LateUpdate()
    {
        if (controller != null)
            controller.AplicarLimiteRenderizacao();
    }
}
