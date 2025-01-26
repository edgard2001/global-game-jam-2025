using System;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // End game if player enters the portal
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has entered the portal");
            
            // Exit application
            Application.Quit();
        }
    }
}
