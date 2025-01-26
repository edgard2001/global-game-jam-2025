using System.Collections;
using System.Diagnostics.Contracts;
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

    private float _speed = 0f;

    private bool _grounded;
    private bool _jumping;
    private bool _dead;
    
    private float _size = 1f;
    private float _targetSizeAfterMerge = 1f;
    private float _temperatureSizeModifier = 0f;
    private float _accelerationSizeModifier = 0f;
    private float _targetAccelerationSizeModifier = 0f;
    
    [SerializeField] private float shrinkSpeed = 0.01f;
    [SerializeField] private float growthSpeed = 0.01f;

    [SerializeField] private Material material;
    [SerializeField] private Color color;
    [SerializeField] private Color coolColor;
    [SerializeField] private Color heatColor;

    private float _lift = 0f;
    
    private Vector3 _scale;
    
    private bool isBoosted = false;
    
    private enum SizeType
    {
        Normal,
        Shrink,
        Grow,
        None
    }

    private SizeType sizeType;
    private float _sizeCooldownTimer;
    
    public Transform _respawnPoint;

    public GameObject deathEffect;
    public GameObject popEffect;
    public GameObject shootEffect;

    private void Start()
    {
        sizeType = SizeType.Normal;
        _sizeCooldownTimer = 0;

        //_platforming = GetComponentInChildren<PlatformingObject>();
        //_platforming.OnTouchGround += OnTouchGround;
        //_platforming.OnLeaveGround += OnLeaveGround;

        _rigidbody = GetComponent<Rigidbody>();
        _playerModelTransform = transform.GetChild(0).transform;
        _cameraTransform = Camera.main.transform;

        Physics.gravity = Vector3.down * (9.81f * gravityMultiplier);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        MoveAndTurn();
        SizeHandler();
    }

    private void SizeHandler()
    {
        if (SizeType.None.Equals(sizeType) && _sizeCooldownTimer > 0)
        {
            _sizeCooldownTimer -= Time.deltaTime;
            Debug.Log("Growing cooldown: " + _sizeCooldownTimer);
            // Change player's size based on the sizeType
            if (_sizeCooldownTimer <= 0)
            {
                sizeType = SizeType.Normal;
            }
        }
        
        switch (sizeType)
        {
            case SizeType.Normal:
                _temperatureSizeModifier = Mathf.Lerp(_temperatureSizeModifier, 0, Time.deltaTime * 2);
                material.color = Color.Lerp(material.color, color, Time.deltaTime * 2);
                _lift = Mathf.Lerp(_lift, 0, Time.deltaTime * 2);
                break;
            case SizeType.Shrink:
                if (_temperatureSizeModifier > 0)
                {
                    _temperatureSizeModifier = Mathf.Lerp(_temperatureSizeModifier, 0, Time.deltaTime * 2);
                    _lift = Mathf.Lerp(_lift, 0, Time.deltaTime * 2);
                }
                _temperatureSizeModifier = Mathf.Clamp(_temperatureSizeModifier - shrinkSpeed * Time.deltaTime, -0.5f * _size, 0.5f * _size); 
                material.color = Color.Lerp(material.color, coolColor, Time.deltaTime);
                break;
            case SizeType.Grow:
                _temperatureSizeModifier = Mathf.Clamp(_temperatureSizeModifier + growthSpeed * Time.deltaTime, -0.5f * _size, 0.5f * _size); 
                material.color = Color.Lerp(material.color, heatColor, Time.deltaTime);
                _lift = Mathf.Clamp(_lift + 10 * Time.deltaTime, 0, 5);
                break;
            case SizeType.None:
                break;
        }

        _size = Mathf.Lerp(_size, _targetSizeAfterMerge, Time.deltaTime * 2);
        transform.localScale = Vector3.one * (_size + _temperatureSizeModifier);
        
        if (Mathf.Abs(_targetAccelerationSizeModifier - _accelerationSizeModifier) > 0.01f)
        {
            _accelerationSizeModifier = Mathf.Lerp(_accelerationSizeModifier, _targetAccelerationSizeModifier, Time.deltaTime * 10f);
            if (Mathf.Abs(_targetAccelerationSizeModifier - _accelerationSizeModifier) < 0.01f)
            {
                _accelerationSizeModifier = _targetAccelerationSizeModifier;
                _targetAccelerationSizeModifier = 0;
            }
        }
        _playerModelTransform.localScale = Vector3.one + 
                                           Vector3.up * _accelerationSizeModifier + 
                                           Vector3.one * (Mathf.Cos(Time.time * (5 + (_speed > 0 ? 5 : 0))) * -0.02f);
        _playerModelTransform.localPosition = Vector3.up * (0.5f + _accelerationSizeModifier / 2);
    }

    public void ShouldShrink()
    {
        sizeType = SizeType.Shrink;
    }
    
    public void ShouldGrow()
    {
        sizeType = SizeType.Grow;
    }
    
    public void EndResizeEffect()
    {
        sizeType = SizeType.None;
        _sizeCooldownTimer = 1;
    }

    private void ProcessInput()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputVector = ClampInput(inputVector);

        if (_dead)
        {
            _speed = 0;
            return;
        }

        bool groundCheck = Physics.Raycast(_playerModelTransform.position, Vector3.down, transform.localScale.x / 2 + 0.01f,1, QueryTriggerInteraction.Ignore);
        if (_grounded != groundCheck)
        {
            _grounded = groundCheck;
            if (_grounded) _targetAccelerationSizeModifier = -0.2f;
        }
        
        _speed = inputVector.magnitude * maxSpeed * (_grounded ? 1 : 1.2f);

        CalculateMovementDirection(inputVector);

        if (_grounded && !_jumping && Input.GetButton("Jump"))
        {
            _jumping = true;
            _targetAccelerationSizeModifier = 0.1f;
        }
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
        
        _rigidbody.linearVelocity += Vector3.up * (_lift * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bubble"))
        {
            float r1 = _size;
            float r2 = other.transform.parent.transform.localScale.x;
            float r3 = Mathf.Pow(r1 * r1 * r1 + r2 * r2 * r2, 1f / 3);
            _targetSizeAfterMerge = r3;
            Destroy(other.transform.parent.gameObject);
        } 
        else if (other.CompareTag("Hazard") && !_dead)
        {
            _dead = true;
            _playerModelTransform.gameObject.SetActive(false);

            GameObject effect1 = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect1, 10f);

            GameObject effect2 = Instantiate(popEffect, transform.position, Quaternion.identity);
            Destroy(effect2, 2f);

            Invoke(nameof(Respawn), 2f);
        }
    }
    
    public void ApplySpeedBoost(float boostAmount)
    {
        Debug.Log("Boosting");
        // Apply force in the player's forward direction
        Vector3 launchDirection = _cameraTransform.forward;// transform.forward;
        _rigidbody.AddForce(launchDirection * boostAmount, ForceMode.Impulse);
    }

    private void Respawn()
    {
        material.color = color;

        _size = 1f;
        _targetSizeAfterMerge = 1f;
        _temperatureSizeModifier = 0f;
        _accelerationSizeModifier = 0f;
        _targetAccelerationSizeModifier = 0f;

        transform.position = _respawnPoint.position;
        _playerModelTransform.gameObject.SetActive(true);

        _dead = false;
    }
}
