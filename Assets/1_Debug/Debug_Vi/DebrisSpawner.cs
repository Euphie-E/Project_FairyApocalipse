using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    private Transform rockDebris;
    private Rigidbody rockDebrisRB;
    public float speed = 2;
    public float size = 2;
    
    void Start()
    {
        rockDebris = transform.GetChild(0);
        rockDebrisRB = rockDebris.GetComponent<Rigidbody>();
        rockDebris.gameObject.SetActive(false);
    }

    public void Drop(bool randomize)
    {
        rockDebris.localPosition = new Vector3(0, 0, 0);

        if (randomize)
        {
            float randomSize = Random.Range(1f, 4f);
            float randomSpeed = Random.Range(0f, 50f);
            rockDebrisRB.AddForce(Vector3.down * randomSpeed, ForceMode.Acceleration);
            rockDebris.localScale = new Vector3(randomSize, randomSize, randomSize);
        }

        else
        {
            rockDebrisRB.AddForce(Vector3.down * speed, ForceMode.Acceleration);
            rockDebris.localScale = new Vector3(size, size, size);
        }

        rockDebris.gameObject.SetActive(true);
    }

}
