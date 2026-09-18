using UnityEngine;

public class PastCameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera pastCamera;
   

    private void LateUpdate()
    {
        pastCamera.transform.SetPositionAndRotation(
            mainCamera.transform.position,
            mainCamera.transform.rotation
        );
    }
}