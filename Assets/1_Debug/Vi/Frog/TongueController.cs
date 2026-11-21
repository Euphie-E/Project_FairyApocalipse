using UnityEngine;

public class TongueController : MonoBehaviour
{
    public Frog frog;
    [SerializeField] Transform tongueBase;
    [SerializeField] Transform[] bones;
    [SerializeField] float maxRange;
    public float lickInterval;
    [SerializeField] float lickDuration;
    [SerializeField] float returnDuration;
    private Vector3 direction;
    public bool isLicking = false;
    private bool isGoing = false;

    float progress = 0;
    
    private Vector3[] targetPositions;
    private Vector3[] originalPositions;

    void Start()
    {
        frog = GetComponent<Frog>();
        originalPositions = new Vector3[bones.Length];
        targetPositions = new Vector3[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            originalPositions[i] = bones[i].position;
        }
    }

    public void StartLick()
    {
        if (isLicking) return;
        transform.rotation = Quaternion.LookRotation(frog.player.position - transform.position);
        direction = (frog.player.position - tongueBase.position).normalized;
        
        /* Debug.Log("player position: " + frog.player.position);
        Debug.Log("Direction: " + direction); */

        //CalculateTargetPositions(frog.player);

        for (int i = 0; i < bones.Length; i++)
        {
            originalPositions[i] = bones[i].position;
        }
        
        for (int i = 0; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            targetPositions[i] = tongueBase.position + direction * maxRange * t;
        }

        progress = 0f;

        isGoing = true;
        isLicking = true;
    }

    /* private void CalculateTargetPositions(Transform player)
    {
        targetPositions = new Vector3[bones.Length];

        Vector3 direction = (player.position - tongueBase.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            targetPositions[i] = tongueBase.position + direction * maxRange * t;
        }
    } */


    void Update()
    {
        if (isLicking) 
        {
            if (isGoing)
            {
                progress += Time.deltaTime / lickDuration;
                progress = Mathf.Clamp01(progress);

                for (int i = 0; i < bones.Length; i++)
                {
                    bones[i].position = Vector3.Lerp(originalPositions[i], targetPositions[i], progress);
                }

                if (progress >= 1f)
                {
                    isGoing = false;
                }

                /* Debug.Log("progress: " + progress); */
            }

            else
            {
                progress -= Time.deltaTime / returnDuration;
                progress = Mathf.Clamp01(progress);

                for (int i = 0; i < bones.Length; i++)
                {
                    bones[i].position = Vector3.Lerp(originalPositions[i], targetPositions[i], progress);
                }

                if (progress <= 0f)
                {
                    isLicking = false;
                }
            }
        }
    }

    /* private void MoveBones(Vector3[] positions, float progress)
    {
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = Vector3.Lerp(originalPositions[i], positions[i], progress);
        }
    } */

    public void EndLick()
    {
        isLicking = false;
    }

    public void OnCollision()
    {
        if (isGoing == true) isGoing = false;
    }

    void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
        #endif
    }
}
