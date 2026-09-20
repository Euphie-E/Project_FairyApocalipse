using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] 
    protected List<GameObject> attachPoints = new List<GameObject>();
    [SerializeField]
    float rotationThreshold = 0.7f;
    [SerializeField]
    float holdThreshold = 1;
    public float forcePS = 0.5f;
    public float start,end;
    virtual public void Attach(InteractController player)
    {
        if(attachPoints.Count <= 0)
        {
            Debug.LogError("attachPoints at "+ this.gameObject.name+" is empty");
            return;
        }
        GameObject closer = attachPoints[0];
        foreach (var item in attachPoints)
        {
            if((item.transform.position - player.transform.position).magnitude <= (closer.transform.position - player.transform.position).magnitude)
            {
                closer = item;
            }
        }
        if (Vector3.Dot(player.transform.forward,closer.transform.forward) >= rotationThreshold)
        {
            Interact(player,closer);
        }
    }
    virtual public void Detach(InteractController player)
    {

    }
    virtual public void Throw()
    {
        Debug.Log(end - start);
    }
    virtual protected void Interact(InteractController player,GameObject attach)
    {
        Debug.Log("interact");
    }
}
