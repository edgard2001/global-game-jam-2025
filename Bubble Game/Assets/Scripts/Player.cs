using System;
using System.Collections;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
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
    private Collider _collider;
    private Transform _playerModelTransform;
    private Transform _cameraTransform;

    private Vector3 _direction = Vector3.forward;

    private float _speed = 0f;

    private bool _grounded;
    private bool _jumping;
    private int _jumpCounter = 0;
    private bool _dead;
    
    private float _size = 1f;
    private float _targetSize = 1f;
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

    private AudioSource _audio;
    public AudioClip jumpAudioClip;
    public AudioClip mergeAudioClip;
    public AudioClip shotAudioClip;
    public AudioClip deathAudioClip;
    public AudioClip growAudioClip;
    public AudioClip shrinkAudioClip;

    public GameObject projectileBubble;

    void Start()
    {
        sizeType = SizeType.Normal;
        _sizeCooldownTimer = 0;

        //_platforming = GetComponentInChildren<PlatformingObject>();
        //_platforming.OnTouchGround += OnTouchGround;
        //_platforming.OnLeaveGround += OnLeaveGround;

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _playerModelTransform = transform.GetChild(0).transform;
        _cameraTransform = Camera.main.transform;
        _audio = GetComponent<AudioSource>();

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
            _sizeCooldownTimer -= Time.fixedDeltaTime;
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
                _temperatureSizeModifier = Mathf.Lerp(_temperatureSizeModifier, 0, Time.fixedDeltaTime * 2);
                material.color = Color.Lerp(material.color, color, Time.fixedDeltaTime * 2);
                _lift = Mathf.Lerp(_lift, 0, Time.fixedDeltaTime * 2);
                break;
            case SizeType.Shrink:
                if (_temperatureSizeModifier > 0)
                {
                    _temperatureSizeModifier = Mathf.Lerp(_temperatureSizeModifier, 0, Time.fixedDeltaTime * 2);
                    _lift = Mathf.Lerp(_lift, 0, Time.fixedDeltaTime * 2);
                }
                _temperatureSizeModifier = Mathf.Clamp(_temperatureSizeModifier - shrinkSpeed * Time.fixedDeltaTime, -0.5f * _size, 0.5f * _size); 
                material.color = Color.Lerp(material.color, coolColor, Time.fixedDeltaTime);
                break;
            case SizeType.Grow:
                _temperatureSizeModifier = Mathf.Clamp(_temperatureSizeModifier + growthSpeed * Time.fixedDeltaTime, -0.5f * _size, 0.5f * _size); 
                material.color = Color.Lerp(material.color, heatColor, Time.fixedDeltaTime);
                _lift = Mathf.Clamp(_lift + 10 * Time.fixedDeltaTime, 0, 5);
                break;
            case SizeType.None:
                _lift = Mathf.Lerp(_lift, 0, Time.fixedDeltaTime * 2);
                break;
        }

        _size = Mathf.Lerp(_size, _targetSize, Time.fixedDeltaTime * 2);
        if (Mathf.Abs(_targetSize - _size) < 0.01) _size = _targetSize;
        transform.localScale = Vector3.one * (_size + _temperatureSizeModifier);
        
        if (Mathf.Abs(_targetAccelerationSizeModifier - _accelerationSizeModifier) > 0.01f)
        {
            _accelerationSizeModifier = Mathf.Lerp(_accelerationSizeModifier, _targetAccelerationSizeModifier, Time.fixedDeltaTime * 10f);
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
        _sizeCooldownTimer = 5;
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

        bool groundCheck = Physics.Raycast(_playerModelTransform.position, Vector3.down, transform.localScale.x / 2 + 0.05f,1, QueryTriggerInteraction.Ignore);
        if (_grounded != groundCheck)
        {
            _grounded = groundCheck;
            if (_grounded)
            {
                _targetAccelerationSizeModifier = -0.2f;
                _jumpCounter = 0;
            }
        }
        
        _speed = inputVector.magnitude * maxSpeed * (_grounded ? 1 : 1.2f);

        CalculateMovementDirection(inputVector);

        if (_grounded && !_jumping && Input.GetButton("Jump") && _targetAccelerationSizeModifier == 0)
        {
            _audio.PlayOneShot(jumpAudioClip);
            _jumping = true;
            _targetAccelerationSizeModifier = 0.1f;
        } 
        
        if (!_grounded)
        {
            _jumping = false;
        }

        if (Input.GetButtonDown("Fire1")) Shoot();
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
        if (_dead) return;
        
        if (_speed > 0 && _grounded)
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
            _jumpCounter++;
        }
        
        _rigidbody.linearVelocity += Vector3.up * (_lift * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dead) return;
        
        if (other.CompareTag("Bubble"))
        {
            float r1 = _size;
            float r2 = other.transform.parent.transform.localScale.x;
            float r3 = Mathf.Pow(r1 * r1 * r1 + r2 * r2 * r2, 1f / 3);
            _targetSize = r3;
            Destroy(other.transform.parent.gameObject);
            _audio.PlayOneShot(mergeAudioClip);
        } 
        else if (other.CompareTag("Hazard") && !_dead)
        {
            print(3);
            KillPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_dead) return;
        
        if (other.CompareTag("PlayerArea") && !_dead)
        {
            print(0);
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        _dead = true;
        _playerModelTransform.gameObject.SetActive(false);
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _collider.enabled = false;

        _audio.PlayOneShot(deathAudioClip);

        GameObject effect1 = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect1, 10f);

        GameObject effect2 = Instantiate(popEffect, transform.position, Quaternion.identity);
        Destroy(effect2, 2f);

        Invoke(nameof(Respawn), 2f);
    }
    
    public void ApplySpeedBoost(float boostAmount)
    {
        Debug.Log("Boosting");
        // Apply force in the player's forward direction
        // Vector3 launchDirection = (boostDirection == SpeedBoost.BoostDirection.CameraForward)? _cameraTransform.forward: transform.up;// transform.forward;
        Vector3 launchDirection = _cameraTransform.forward;
        _rigidbody.AddForce(launchDirection * boostAmount, ForceMode.Impulse);
    }

    private void Respawn()
    {
        print(2);
        material.color = color;

        _size = 1f;
        _targetSize = 1f;
        _temperatureSizeModifier = 0f;
        _accelerationSizeModifier = 0f;
        _targetAccelerationSizeModifier = 0f;

        _collider.enabled = true;
        
        transform.position = _respawnPoint.position;
        _playerModelTransform.gameObject.SetActive(true);
        
        _rigidbody.useGravity = true;
        _rigidbody.linearVelocity = Vector3.zero;

        _dead = false;
    }

    private void Shoot()
    {
        if (_size < 1.03f) return;
            
        _targetSize = _size - 0.03f;

        Vector3 force = _cameraTransform.forward * 10 + Vector3.up * 5;

        GameObject projectile = Instantiate(projectileBubble, 
            _playerModelTransform.position + _cameraTransform.forward * transform.localScale.z,
            Quaternion.LookRotation(force)
        );
        projectile.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        Destroy(projectile, 5f);
        _audio.PlayOneShot(shotAudioClip);

    }
}
