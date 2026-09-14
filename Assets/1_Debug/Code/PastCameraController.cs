using UnityEngine;

public class PastCameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera pastCamera;
    [SerializeField] private Camera pastDepthCamera;

    private void LateUpdate()
    {
        pastCamera.transform.SetPositionAndRotation(
            mainCamera.transform.position,
            mainCamera.transform.rotation
        );

        pastDepthCamera.transform.SetPositionAndRotation(
            mainCamera.transform.position,
            mainCamera.transform.rotation
        );
    }
}