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
    private bool isGoing = false;

    float progress = 0;

    Vector3[] startPositions;
    
    private Vector3[] returnStartPositions;
    private Vector3[] targetPositions;
    private Vector3[] originalPositions;
    private Vector3 end;

    void Start()
    {
        frog = GetComponent<Frog>();
        originalPositions = new Vector3[bones.Length];
        targetPositions = new Vector3[bones.Length];
        returnStartPositions = new Vector3[bones.Length];

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
        
        Debug.Log("player position: " + frog.player.position);
        Debug.Log("Direction: " + direction);

        //CalculateTargetPositions(frog.player);
        
        for (int i = 0; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            targetPositions[i] = tongueBase.position + direction * maxDistance * t;
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

            targetPositions[i] = tongueBase.position + direction * maxDistance * t;
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
                    for (int i = 0; i < bones.Length; i++)
                    {
                        returnStartPositions[i] = bones[i].position;
                    }
                    isGoing = false;
                }

                Debug.Log("progress: " + progress);
            }

            else
            {
                progress += Time.deltaTime / returnDuration;
                progress = Mathf.Clamp01(progress);

                for (int i = 0; i < bones.Length; i++)
                {
                    bones[i].position = Vector3.Lerp(returnStartPositions[i], originalPositions[i], progress);
                }

                if (progress >= 1f)
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
}
