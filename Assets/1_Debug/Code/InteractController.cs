using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractController : MonoBehaviour
{
    
    
    public Interactable atteched = null;
    [SerializeField] 
    float radiusCheck = 1;
    [SerializeField]
    bool gizmo = false;
    Collider[] list = new Collider[10];
    void Awake()
    {
        
        
    }
    void Start()
    {
        PlayerInput.Instance.AddAction(Press,3);
        PlayerInput.Instance.AddCancelAction(Cancel,3);
    }

    void Press()
    {
        if (atteched == null)
        {
            int max = Physics.OverlapSphereNonAlloc(this.transform.position,radiusCheck,list);
            if(gizmo) DrawDebugSphere(transform.position,radiusCheck,Color.blue,2);
            for(int i = 0; i<max;i++)
            {
                if (list[i].CompareTag("Interactable"))
                {
                    list[i].GetComponent<Interactable>().Attach(this);
                    break;
                }
            }
        }
        else
        {
            atteched.start = Time.time;
            atteched.Detach(this);
        }
    }
    void Cancel()
    {
        if (atteched != null)
        {
            atteched.end = Time.time;
            atteched.Throw();
        }
    }
    public static void DrawDebugSphere(Vector3 center, float radius, Color color, float duration)
    {
        // Draws a 3D crosshair showing the sphere bounds
        Debug.DrawLine(center + Vector3.left * radius, center + Vector3.right * radius, color, duration);
        Debug.DrawLine(center + Vector3.up * radius, center + Vector3.down * radius, color, duration);
        Debug.DrawLine(center + Vector3.forward * radius, center + Vector3.back * radius, color, duration);
    }
}
