using UnityEngine;

public class TongueController : MonoBehaviour
{
    public Frog frog;
    [SerializeField] Transform tongueBase;
    [SerializeField] Transform[] bones;
    [SerializeField] float maxDistance;
    public float lickInterval;
    [SerializeField] float lickDuration;
    [SerializeField] float returnDuration;
    private Vector3 direction;
    public bool isLicking = false;

    void Start()
    {
        frog = GetComponent<Frog>();
    }

    public void StartLick()
    {
        isLicking = true;
        direction = (frog.player.position - tongueBase.position).normalized;
        Vector3 target = tongueBase.position + direction * maxDistance;
        for (int i = 0; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            Vector3 targetPosition = Vector3.Lerp(
                tongueBase.position,
                target,
                t
            );
        }
    }

    public void EndLick()
    {
        isLicking = false;
    }
}
