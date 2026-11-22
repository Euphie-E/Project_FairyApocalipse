using UnityEngine;

public class CutsceneStarter : MonoBehaviour
{
    public bool hasPlayed = false;

    [SerializeField] private CutsceneManager.CutsceneType cutsceneType;
    [SerializeField] private int midgameCutsceneID;

    private void OnTriggerStay(Collider other)
    {
        if (hasPlayed)
            return;

        if (other.CompareTag("Player"))
        {
            if (PlayerStateController.Instance.playerMovement.isGrounded)
            {
                hasPlayed = true;
                switch (cutsceneType)
                {
                    case CutsceneManager.CutsceneType.Starting:
                        CutsceneManager.Instance.PlayCutscene(CutsceneManager.CutsceneType.Starting);
                        break;
                    case CutsceneManager.CutsceneType.Midgame:
                        CutsceneManager.Instance.PlayCutscene(midgameCutsceneID);
                        break;
                    case CutsceneManager.CutsceneType.Ending: 
                        CutsceneManager.Instance.PlayCutscene(CutsceneManager.CutsceneType.Ending);
                        break;
                }
            }
        }
    }
}
