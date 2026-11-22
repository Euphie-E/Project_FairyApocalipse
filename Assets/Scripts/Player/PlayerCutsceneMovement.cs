using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCutsceneMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -25f;

    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity += gravity * Time.deltaTime;

        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
