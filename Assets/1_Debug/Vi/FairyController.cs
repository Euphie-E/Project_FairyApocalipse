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
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

            // rotação
            Vector3 direction = target.forward;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                distance = Vector3.Distance(transform.position, target.position);

                Quaternion rotation = Quaternion.LookRotation(direction);

                if (distance > lieDownDistance)
                {
                    rotation *= Quaternion.Euler(lieDownAngle, 0f, 0f);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (distance < 0.8f && target == seedTarget)
            {
                animator.SetBool("isHolding", true);
            }
        }
        
    }
}
