using UnityEngine;

public class Grabbable : Interactable
{
    protected override void Interact(InteractController player,GameObject attach)
    {
        //AttachPlayer not real implemented
        player.transform.SetPositionAndRotation(attach.transform.position, attach.transform.rotation);
        player.atteched = this;
        this.transform.SetParent(player.transform,true);
    }
    public override void Detach(InteractController player)
    {
        this.transform.SetParent(null);
        player.atteched = null;
    }
}
