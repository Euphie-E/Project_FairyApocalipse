using System;
using UnityEngine;

public class FairyMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float lieDownDistance = 1f;
    [SerializeField] private float lieDownAngle = 25f;
    private float distance = 0;

    void LateUpdate()
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
    }
}
