using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SwingBarController : MonoBehaviour
{
    [SerializeField] float grabDuration = 0.25f;
    [SerializeField] float grabHeight = 2f;
    [SerializeField] Transform swingPivot;
    [SerializeField] float timeToReattach = 1f;
    [SerializeField] float edgeMargin = 0.2f;

    public bool playerAttached;
    public CinemachineCamera cam;
    private bool canAttach = true;
    private CharacterController controller;
    private BoxCollider boxCollider;

    Transform playerFather;
    Transform playerTrans;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        controller = other.GetComponent<CharacterController>();

        if (controller != null && other.CompareTag("Player"))
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

        //Coloca o pivot de rota��o proximo ao local que o player encostou.
        Vector3 contactPoint = boxCollider.ClosestPoint(player.position);

        Vector3 localContactPoint = boxCollider.transform.InverseTransformPoint(contactPoint);

        Vector3 pivotPosition = swingPivot.localPosition;

        float halfWidth = boxCollider.size.x * 0.5f;
        float minX = -halfWidth + edgeMargin;
        float maxX = halfWidth - edgeMargin;

        pivotPosition.x = Mathf.Clamp(localContactPoint.x, minX, maxX);

        swingPivot.localPosition = pivotPosition;

        //Verifica se Player est� na frente ou atr�s.
        float dot = Vector3.Dot(player.forward, transform.forward);
        bool isFront = dot > 0f;

        //Pega a velocidade do player ao escostar na barra
        Vector3 velocity = playerController.velocity;
        float forwardSpeed = Vector3.Dot(velocity, swingPivot.forward);

        playerController.enabled = false;


        playerTrans = player;
        playerFather = player.parent;
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
        playerTrans.SetParent(playerFather,true);
        swingPivot.rotation = new Quaternion(0f , 0f, 0f, 0f);
        StartCoroutine(ResetAttach());
    }

    IEnumerator ResetAttach()
    {
        yield return new WaitForSeconds(timeToReattach);
        canAttach = true;
    }
}
