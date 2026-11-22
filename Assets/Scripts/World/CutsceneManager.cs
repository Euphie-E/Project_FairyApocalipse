using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("Starting")]
    [SerializeField] private PlayableDirector startingCutscene;

    [Header("MidGame")]
    [SerializeField] private PlayableDirector[] midgameCutscenes;

    [Header("Ending")]
    [SerializeField] private PlayableDirector endingCutscene;

    public enum CutsceneType
    {
        Starting,
        Midgame,
        Ending
    }

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void PlayStartingCutscene()
    {
        CutsceneStartSettings();

        if (startingCutscene == null)
            return;

        startingCutscene.Play();
    }

    void PlayEndingCutscene()
    {
        CutsceneStartSettings();

        if (endingCutscene == null)
            return;

        endingCutscene.Play();
    }

    public void PlayCutscene(int cutsceneID)
    {
        CutsceneStartSettings();

        if (cutsceneID < 0 || cutsceneID >= midgameCutscenes.Length)
            return;

        midgameCutscenes[cutsceneID].Play();
    }

    public void PlayCutscene(CutsceneType type)
    {
        switch (type)
        {
            case CutsceneType.Starting:
                PlayStartingCutscene();
                break;
            case CutsceneType.Ending:
                PlayEndingCutscene();
                break;
        }
    }

    private void CutsceneStartSettings()
    {
        PlayerStateController.Instance.StartCutsceneState();
    }

    public void EndCutscene()
    {
        PlayerStateController.Instance.StartMovement();
    }
}
