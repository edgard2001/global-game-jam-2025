using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float inertia = 2;
    [SerializeField] private float jumpForceMultiplier = 40000;
    [SerializeField] private float gravityMultiplier = 10;
    [SerializeField] private float dashForceMultiplier = 20000f;

    [Header("Input")]
    [SerializeField] private float controllerStickDeadZone = 0.2f;

    //private PlatformingObject _platforming;

    private Rigidbody _rigidbody;
    private Transform _playerModelTransform;
    private Transform _cameraTransform;

    private Vector3 _direction = Vector3.forward;

    private float _speed;

    private bool _grounded;
    private bool _jumping;
    private bool _dead;
    public Transform _respawnPoint;

    public bool _scheduledJump;

    private void Start()
    {
        //_platforming = GetComponentInChildren<PlatformingObject>();
        //_platforming.OnTouchGround += OnTouchGround;
        //_platforming.OnLeaveGround += OnLeaveGround;

        _rigidbody = GetComponent<Rigidbody>();
        _playerModelTransform = transform.GetChild(0).transform;
        _cameraTransform = Camera.main.transform;

        Physics.gravity = Vector3.down * (9.81f * gravityMultiplier);
    }

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        MoveAndTurn();
    }

    private void ProcessInput()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputVector = ClampInput(inputVector);

        _grounded = Physics.Raycast(_playerModelTransform.position, Vector3.down, transform.localScale.x / 2 + 0.01f);
        _speed = inputVector.magnitude * maxSpeed * (_grounded ? 1 : 1.2f);

        CalculateMovementDirection(inputVector);

        bool nearGround = Physics.Raycast(_playerModelTransform.position, Vector3.down, transform.localScale.x / 2 + 1f);
        if (_grounded && Input.GetButton("Jump"))
        {
            _jumping = true;
           // _scheduledJump = false;
        }//else if (_grounded && _scheduledJump)
        //{
          //  _jumping = true;
           // _scheduledJump = false;
       // } //else if (nearGround && _rigidbody.linearVelocity.y < 0 && Input.GetButton("Jump"))
        //{
        //    _scheduledJump = true;
        //}
    }

    private Vector2 ClampInput(Vector2 inputVector)
    {
        if (Mathf.Abs(inputVector.x) < controllerStickDeadZone) inputVector.x = 0;
        if (Mathf.Abs(inputVector.y) < controllerStickDeadZone) inputVector.y = 0;
        if (inputVector.sqrMagnitude > 1) inputVector.Normalize();
        return inputVector;
    }

    private void CalculateMovementDirection(Vector2 inputVector)
    {
        if (inputVector.sqrMagnitude == 0)
            return;

        _direction = Vector3.zero;

        Vector3 cameraRelativeForward = _cameraTransform.forward;
        cameraRelativeForward.y = 0;
        cameraRelativeForward.Normalize();

        Vector3 cameraRelativeRight = _cameraTransform.right;
        cameraRelativeRight.y = 0;
        cameraRelativeRight.Normalize();

        _direction = cameraRelativeRight * inputVector.x + cameraRelativeForward * inputVector.y;
    }

    private void MoveAndTurn()
    {
        if (_speed > 0 && _grounded && !_dead)
            _playerModelTransform.rotation = Quaternion.Lerp(_playerModelTransform.rotation,
                Quaternion.LookRotation(_direction),
                10f * Time.fixedDeltaTime);

        _rigidbody.linearVelocity = Vector3.Lerp(
            _rigidbody.linearVelocity, 
            _direction * _speed + Vector3.up * _rigidbody.linearVelocity.y, 
            Time.fixedDeltaTime * 1 / inertia
            );

        if (_jumping)
        {
            _rigidbody.AddForce(Vector3.up * jumpForceMultiplier);
            _jumping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bubble"))
        {
            float r1 = transform.localScale.x;
            float r2 = other.transform.parent.transform.localScale.x;
            float r3 = Mathf.Pow(r1 * r1 * r1 + r2 * r2 * r2, 1f / 3);
            transform.localScale = Vector3.one * r3;
            Destroy(other.transform.parent.gameObject);
        } else if (other.CompareTag("Hazard"))
        {
            transform.position = _respawnPoint.position;
        }
    }
}
