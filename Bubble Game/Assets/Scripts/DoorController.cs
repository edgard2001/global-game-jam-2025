using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Socket activationSwitch;
    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;
    
    private void Awake()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + new Vector3(0, _closedPosition.y - 3, 0);
        
        if (activationSwitch)
        {
            activationSwitch.OnActivate += OpenDoor;
            activationSwitch.OnDeactivate += CloseDoor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Control door movement
        if (_isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, _openPosition, Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _closedPosition, Time.deltaTime);
        }
    }
    
    // Call this method to unlock the door
    public void OpenDoor()
    {
        _isOpen = true;
    }

    public void CloseDoor()
    {
        _isOpen = false;
    }
}
