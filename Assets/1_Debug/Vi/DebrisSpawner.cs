using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebrisSpawner : MonoBehaviour
{
    private Transform rockDebris;
    private Rigidbody rockDebrisRB;
    public float speed = 2;
    public float size = 2;
    
    private ShadowEffect shadowEffect;

    void Start()
    {
        
        rockDebris = transform.GetChild(0);
        rockDebrisRB = rockDebris.GetComponent<Rigidbody>();
        rockDebris.gameObject.SetActive(false);

        shadowEffect = rockDebris.transform.GetChild(0).GetComponent<ShadowEffect>();
    }

    public void Drop(float startHeight, bool randomize)
    {
        rockDebrisRB.linearVelocity = Vector3.zero;
        rockDebris.gameObject.SetActive(false);
        rockDebris.localPosition = new Vector3(0, 0, 0);
        rockDebris.gameObject.SetActive(true);
        shadowEffect.StartDrop(startHeight);

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
