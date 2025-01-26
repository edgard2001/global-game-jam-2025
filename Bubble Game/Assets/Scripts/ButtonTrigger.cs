using System;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour, IInteractableTrigger
{
    public event Action OnActivate;
    public event Action OnDeactivate;

    public Material platformMaterial;
    public Material platformPressedMaterial;
    
    [SerializeField] private bool toggleMode = false; // New toggle mode boolean
    private bool _isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            if (!toggleMode) // Original behavior
            {
                if (!_isPressed)
                {
                    ChangeMaterial(platformPressedMaterial);
                    _isPressed = true;
                    OnActivate?.Invoke();
                }
            } 
            else // Toggle mode behavior
            {
                _isPressed = !_isPressed;
                
                if (_isPressed)
                {
                    ChangeMaterial(platformPressedMaterial);
                    OnActivate?.Invoke();
                }
                else
                {
                    ChangeMaterial(platformMaterial);
                    OnDeactivate?.Invoke();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!toggleMode && _isPressed && other.CompareTag("Player"))
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