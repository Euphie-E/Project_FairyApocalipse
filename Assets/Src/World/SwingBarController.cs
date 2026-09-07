using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SwingBarController : MonoBehaviour
{
    [SerializeField] float grabDuration = 0.25f;
    [SerializeField] float grabHeight = 2f;
    [SerializeField] Transform swingPivot;
    [SerializeField] float timeToReattach = 1f;

    public bool playerAttached;
    public CinemachineCamera cam;
    private bool canAttach = true;
    private CharacterController controller;

    private void OnTriggerEnter(Collider other)
    {
        controller = other.GetComponent<CharacterController>();

        if (controller != null)
        {
            if (playerAttached)
                return;

            StartCoroutine(AttachPlayer(controller));
        }
    }

    private IEnumerator AttachPlayer(CharacterController playerController)
    {
        if (!canAttach)
            yield break;

        playerAttached = true;

        Transform player = playerController.transform;

        float dot = Vector3.Dot(player.forward, transform.forward);

        bool isFront = dot > 0f;

        Vector3 velocity = playerController.velocity;
        float forwardSpeed = Vector3.Dot(velocity, swingPivot.forward);

        playerController.enabled = false;

        player.SetParent(swingPivot, true);

        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;

        Vector3 targetPosition = swingPivot.position + transform.up * grabHeight;

        Quaternion targetRotation;

        if (isFront)
        {
            targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        }

        float timer = 0f;

        while (timer < grabDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / grabDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            player.position = Vector3.Lerp(startPosition, targetPosition, t);

            player.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        player.position = targetPosition;
        player.rotation = targetRotation;

        PlayerStateController.Instance.StartSwing(swingPivot, this, forwardSpeed);
        
        canAttach = false;
    }

    public void ResetSwingBar()
    {
        playerAttached = false;
        swingPivot.rotation = new Quaternion(0f , 0f, 0f, 0f);
        StartCoroutine(ResetAttach());
    }

    IEnumerator ResetAttach()
    {
        yield return new WaitForSeconds(timeToReattach);
        canAttach = true;
    }
}
