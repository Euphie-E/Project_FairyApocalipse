using Unity.VisualScripting;
using UnityEngine;

public class EartquakeController : MonoBehaviour
{
    private DebrisSpawner[] spawners;
    [SerializeField] private GameObject rockDebris;

    [SerializeField] private bool autoEarthquake = false;

    [SerializeField] private float earthquakeInterval = 5;
    [SerializeField] private bool randomize = false;
    private float earthquakeTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        earthquakeTimer = earthquakeInterval;

        spawners = new DebrisSpawner[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            spawners[i] = transform.GetChild(i).GetComponent<DebrisSpawner>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleAutoEarthquake();
    }

    private void HandleAutoEarthquake()
    {
        if (autoEarthquake)
        {
            earthquakeTimer -= Time.deltaTime;
            if (earthquakeTimer <= 0)
            {
                StartEarthquake(true);
            }
        }  
    }

    public void StartAutoEarthquakes()
    {
        autoEarthquake = true;
        earthquakeTimer = earthquakeInterval;
    }

    public void StopEarthquakes()
    {
        autoEarthquake = false;
        earthquakeTimer = earthquakeInterval;
    }

    public void StartEarthquake(bool resetTimer)
    {
        if (resetTimer)
        {
            earthquakeTimer = earthquakeInterval;
        }

        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].Drop(randomize);
        }


    }
}
