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

    private Transform[] originalTransforms;
    private Vector3 end;

    void Start()
    {
        frog = GetComponent<Frog>();
        originalTransforms = bones;
    }

    public void StartLick()
    {
        if (isLicking) return;
        direction = (frog.player.position - tongueBase.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
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
        isLicking = true;
    }

    void Update()
    {
        if (isLicking) Lick();
    }

    void Lick()
    {
        /* Vector3 direction = (frog.player.position - tongueBase.position).normalized;

        end = tongueBase.position + direction * maxDistance;

        Vector3[] targets = new Vector3[bones.Length];

        // Bone 2 vai para a saída da boca
        targets[0] = tongueBase.position;

        // Bones 3 até 10 são distribuídos em linha reta
        for (int i = 1; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            targets[i] = Vector3.Lerp(tongueBase.position, end, t);
        }

        yield return MoveToPositions(targets, extendDuration, true);

        // Sempre retrai, independentemente do motivo
        yield return Retract();

        isAttacking = false; */
    }


    public void EndLick()
    {
        isLicking = false;
    }
}
