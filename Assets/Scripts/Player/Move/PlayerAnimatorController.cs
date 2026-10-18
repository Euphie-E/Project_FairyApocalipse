using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private static readonly int SwingDirHash = Animator.StringToHash("SwingDir");
    public Animator animator;

    private PlayerMovement movement;
    private PlayerSwingController swing;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        swing = GetComponent<PlayerSwingController>();
    }

    [SerializeField] float dumpTime = 0.5f;
    private void Update()
    {
        //if (movement.enabled) UpdateMovementAnimation();

        //else if (swing.enabled) UpdateSwingAnimation();

        float speed = movement.horizontalVelocity.magnitude;
        bool isGrounded = movement.isGrounded;
        bool isSwinging = swing.isSwinging;
        float swingDir = swing.swingDir;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("VerticalSpeed", movement.verticalVelocity, 0.1f, Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSwinging", isSwinging);

        if (isSwinging)
        {
            if (Vector3.Dot(swing.currentBar.transform.forward, movement.transform.forward) < 0)
                swingDir = -swingDir;

            if (swingDir == 0 && animator.GetFloat(SwingDirHash) < 0.1f && animator.GetFloat(SwingDirHash) > -0.1f)
                animator.SetFloat(SwingDirHash, swingDir);
            else animator.SetFloat(SwingDirHash, swingDir, dumpTime, Time.deltaTime);
        }
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

    public void PlayDash()
    {
        animator.SetTrigger("Dash");
    }
}
