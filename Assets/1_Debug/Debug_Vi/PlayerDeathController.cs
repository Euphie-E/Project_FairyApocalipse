using System;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private bool falling = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();

    }

    void Update()
    {
        
    }
}
