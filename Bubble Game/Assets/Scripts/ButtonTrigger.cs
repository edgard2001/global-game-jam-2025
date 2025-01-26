using System;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour, IInteractableTrigger
{
    public event Action OnActivate;
    public event Action OnDeactivate;

    public Material platformMaterial;
    public Material platformPressedMaterial;
    
    private bool _isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isPressed && other.CompareTag("Player"))
        {
            ChangeMaterial(platformPressedMaterial);
            _isPressed = true;
            OnActivate?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isPressed && other.CompareTag("Player"))
        {
            ChangeMaterial(platformMaterial);
            _isPressed = false;
            OnDeactivate?.Invoke();
        }
    }
    
    private void ChangeMaterial(Material material)
    {
        gameObject.GetComponent<Renderer>().material = material;
    }
}