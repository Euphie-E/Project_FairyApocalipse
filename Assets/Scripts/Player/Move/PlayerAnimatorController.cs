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
        float swingDir = swing.swingDir;
        bool isGrounded = movement.isGrounded;
        bool isSwinging = swing.isSwinging;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("VerticalSpeed", movement.verticalVelocity, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSwinging", isSwinging);
        

        if(swingDir == 0 && animator.GetFloat("SwingDir")<0.1f && animator.GetFloat("SwingDir") > -0.1f)
            animator.SetFloat("SwingDir",swingDir);
        else animator.SetFloat("SwingDir",swingDir,0.1f,Time.deltaTime);
            
    }

    private void UpdateMovementAnimation()
    {
        float speed = movement.horizontalVelocity.magnitude;
        bool isGrounded = movement.isGrounded;
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
