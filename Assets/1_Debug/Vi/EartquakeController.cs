using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EartquakeController : MonoBehaviour
{
    private DebrisSpawner[] spawners;
    private Transform player;
    [SerializeField] private GameObject rockDebris;
    [SerializeField] private int debrisQuantity = 3;
    [SerializeField] private bool autoEarthquake = false;

    [SerializeField] private float earthquakeInterval = 5;
    [SerializeField] private bool randomize = false;
    private float earthquakeTimer;

    [Header("Spawn Position")]
    [SerializeField] private float height = 25;
    [SerializeField] private float range = 5;
    void Start()
    {
        /* try // Quando tirar o renderer do prefab não precisa mais disso
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        catch
        {
            Debug.LogError("não tem renderer, pode tirar esse try catch");
        } */
        player = PlayerInput.Instance.transform;

        earthquakeTimer = earthquakeInterval;

        spawners = new DebrisSpawner[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            spawners[i] = transform.GetChild(i).GetComponent<DebrisSpawner>();
        }

        PlayerInput.Instance.AddAction(TestEarthquake, 7);
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

        for (int i = 0; i < (debrisQuantity > spawners.Length ? spawners.Length : debrisQuantity); i++)
        {
            float x = player.position.x + UnityEngine.Random.Range(-range, range);
            float y = player.position.y + height;
            float z = player.position.z + UnityEngine.Random.Range(-range, range);

            Vector3 newPosition = new Vector3(x, y, z);

            spawners[i].transform.position = newPosition;
            spawners[i].Drop(height, randomize);
        }
    }

    public void TestEarthquake(InputAction.CallbackContext ctx)
    {
        StartEarthquake(false);
    }
}
