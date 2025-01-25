using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject triggerPlatform;
    public Material platformMaterial;
    public Material platformPressedMaterial;
    public bool isPressed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // platformMaterial = Resources.Load("materials/PlatformDefaultMaterial") as Material;
        // platformPressedMaterial = Resources.Load("materials/PlatformPressedMaterial") as Material;
        
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !isPressed)
        {
            Debug.Log("Player has entered the trigger");
            // Set platform material to pressed material
            ChangeMaterial(platformPressedMaterial);
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isPressed)
        {
            Debug.Log("Player has left the trigger");
            // Set platform material to default material
            ChangeMaterial(platformMaterial);
            isPressed = false;
        }
    }
    
    private void ChangeMaterial(Material material)
    {
        triggerPlatform.GetComponent<Renderer>().material = material;
    }
}
