// Assets/Scripts/DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour activationSwitch;
    private IInteractableTrigger _interactableTrigger;
    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;

    private void Awake()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + new Vector3(0, _closedPosition.y - 4, 0);

        _interactableTrigger = activationSwitch as IInteractableTrigger;
        if (_interactableTrigger != null)
        {
            _interactableTrigger.OnActivate += OpenDoor;
            _interactableTrigger.OnDeactivate += CloseDoor;
        }
    }

    void Update()
    {
        if (_isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, _openPosition, Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _closedPosition, (float)(Time.deltaTime * 0.5));
        }
    }

    public void OpenDoor()
    {
        _isOpen = true;
    }

    public void CloseDoor()
    {
        // Switch to false after 1 second
        Invoke("CloseDoorAfterDelay", 1);
    }
    
    private void CloseDoorAfterDelay()
    {
        _isOpen = false;
    }
}
