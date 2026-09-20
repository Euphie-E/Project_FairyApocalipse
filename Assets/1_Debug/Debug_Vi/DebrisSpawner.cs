using System;
using System.Linq.Expressions;
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
        try // Quando tirar o renderer do prefab não precisa mais disso
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        catch
        {
            Debug.LogError("não tem renderer, pode tirar esse try catch");
        }
        
        rockDebris = transform.GetChild(0);
        rockDebrisRB = rockDebris.GetComponent<Rigidbody>();
        rockDebris.gameObject.SetActive(false);
    }

    public void Drop(bool randomize)
    {
        rockDebrisRB.linearVelocity = Vector3.zero;
        rockDebris.localPosition = new Vector3(0, 0, 0);

        if (randomize)
        {
            float randomSize = UnityEngine.Random.Range(1f, 4f);
            float randomSpeed = UnityEngine.Random.Range(0f, 20f) * 100;
            rockDebrisRB.AddForce(Vector3.down * randomSpeed, ForceMode.Acceleration);
            rockDebris.localScale = Vector3.one * randomSize;
        }

        else
        {
            rockDebrisRB.AddForce(Vector3.down * speed, ForceMode.Acceleration);
            rockDebris.localScale = Vector3.one * size;
        }

        rockDebris.gameObject.SetActive(true);
    }

}
