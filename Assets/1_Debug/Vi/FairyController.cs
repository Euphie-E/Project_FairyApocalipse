using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class FairyController : MonoBehaviour
{
    public static FairyController Instance;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform fairySeed;
    [SerializeField] private Transform target;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private Transform seedTarget;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float lieDownDistance = 1f;
    [SerializeField] private float lieDownAngle = 25f;
    [SerializeField] private float pickingDistance = 1f;
    [SerializeField] private float pickingSpeedMultiplier = 1.2f;
    [SerializeField] private float dashSpeed = 2f;
    private float speedMultiplier = 1f;
    private bool carringSeed = false;
    private float distance = 0;

    /// <summary>
    /// 0 - Follow
    /// 1 - Pick
    /// </summary>
    private float state = 0;

    public void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PickSeed(Transform seed)
    {
        if (seedTarget == null)
        {
            seedTarget = seed;
            state = 1;
        }
        
    }

    public void GrabSeed()
    {
        state = 0;
        seedTarget.GetComponent<Collider>().enabled = false;
        seedTarget.GetComponent<Collectible>().enabled = false;
        seedTarget.localScale *= 0.5f;
        seedTarget.SetParent(fairySeed);
        seedTarget.gameObject.layer = 0;
        seedTarget.GetChild(0).gameObject.layer = 0;
        seedTarget.localPosition = Vector3.zero;
        seedTarget.localRotation = quaternion.Euler(90, 0 ,0);
        carringSeed = true;
    }

    public void UseSeed()
    {
        Destroy(seedTarget.gameObject);
        seedTarget = null;
        carringSeed = false;
        animator.SetBool("isHolding", false);
    }

    void LateUpdate()
    {
        if (state == 0)
        {
            // conferindo se vai pro ombro direito ou esquerdo
            if (Vector3.Distance(transform.position, leftTarget.position) > Vector3.Distance(transform.position, rightTarget.position))
            {
                target = rightTarget;
            }
            
            else
            {
                target = leftTarget;
            }
        }

        else
        {
            target = seedTarget;
        }

        if (carringSeed)
        {
            seedTarget.gameObject.layer = 0;
            seedTarget.localPosition = Vector3.up * 0.003f;
            seedTarget.localRotation = quaternion.Euler(90, 0 ,0);
        }

        if (target != null)
        {
            // position
            Vector3 pos = transform.position;
            Vector3 targetPos = target.position;

            pos.y = 0;
            targetPos.y = 0;

            distance = Vector3.Distance(pos, targetPos);

            if (distance > 2f && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") && !animator.IsInTransition(0))
            {
                animator.SetTrigger("Dash");
                Debug.Log("triggei animação");
            }

            speedMultiplier = animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") ? dashSpeed : 1;
            speedMultiplier *= state == 1 ? pickingSpeedMultiplier : 1;
            speedMultiplier *= Mathf.Max(distance/1.8f, 1f);
            
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * speedMultiplier * Time.deltaTime);

            // rotação
            Vector3 direction = target.forward;
            direction.y = 0;

            if (direction.magnitude > 0.5f)
            {
                distance = Vector3.Distance(transform.position, target.position);

                Quaternion rotation = Quaternion.LookRotation(direction);

                if (distance > lieDownDistance)
                {
                    rotation *= Quaternion.Euler(lieDownAngle, 0f, 0f);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (distance < pickingDistance && target == seedTarget)
            {
                animator.SetBool("isHolding", true);
            }
        }
    }
}
