using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    private PlayerMovement movement;
    private PlayerSwingController swing;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        swing = GetComponent<PlayerSwingController>();
    }

    private void Update()
    {
        //if (movement.enabled) UpdateMovementAnimation();

        //else if (swing.enabled) UpdateSwingAnimation();

        float speed = movement.horizontalVelocity.magnitude;
        bool isGrounded = movement.characterController.isGrounded;
        bool isSwinging = swing.isSwinging;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("VerticalSpeed", movement.verticalVelocity, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSwinging", isSwinging);
    }

    private void UpdateMovementAnimation()
    {
        float speed = movement.horizontalVelocity.magnitude;
        bool isGrounded = movement.characterController.isGrounded;
        bool isSwinging = swing.isSwinging;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("VerticalSpeed", movement.verticalVelocity, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        
    }

    private void UpdateSwingAnimation()
    {
        animator.SetBool("isSwinging", true);
    }

    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }
}
