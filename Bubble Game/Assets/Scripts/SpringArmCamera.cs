using UnityEngine;

public class SpringArmCamera : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private Vector3 offset = Vector3.up;
    [SerializeField] private float cameraChaseSpeed = 4f;

    [Header("Collision")]
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float maxDistance = 10;
    [SerializeField] private float sphereCheckRadius = 0.1f;
    [SerializeField] private LayerMask layerMask;

    [Header("Input")]
    [SerializeField] private float lookSensitivityMultiplier = 200;

    [Header("Target")]
    [SerializeField] private Transform playerTransform;
    private Transform _pivotTransform;
    private Transform _cameraTransform;

    private Quaternion _startRotation;
    private float _pitch;
    private float _yaw;


    [SerializeField] private float cameraResetCooldown = 1;
    private float _cameraResetTime;

    private void Start()
    {

        _pivotTransform = transform;
        _cameraTransform = GetComponentInChildren<UnityEngine.Camera>().transform;

        _startRotation = _pivotTransform.rotation;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(horizontalInput) < 0.1 && Mathf.Abs(verticalInput) < 0.1)
        {
            _cameraResetTime -= Time.deltaTime;
            if (_cameraResetTime <= 0)
            {
                // float targetPitch = Vector2.Angle(new Vector2(_pivotTransform.forward.z,_pivotTransform.forward.y).normalized, new Vector2(_playerTransform.forward.z,_playerTransform.forward.y).normalized);
                // if (targetPitch > 180) targetPitch -= 360;
                // _pitch = Mathf.Lerp(_pitch, targetPitch, 1f * Time.deltaTime);
                //
                // float targetYaw = Vector2.Angle(new Vector2(_pivotTransform.forward.x,_pivotTransform.forward.z).normalized, new Vector2(_playerTransform.forward.x,_playerTransform.forward.z).normalized);
                // if (targetYaw > 180) targetYaw -= 360;
                // _yaw= Mathf.Lerp(_yaw, targetYaw, 1f * Time.deltaTime);
                //
                // _pivotTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0) * _startRotation;
            }

            return;
        }

        _cameraResetTime = cameraResetCooldown;

        _pitch -= verticalInput * lookSensitivityMultiplier * Time.deltaTime;
        _yaw += horizontalInput * lookSensitivityMultiplier * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -80, 74);
    }

    private void LateUpdate()
    {
        _pivotTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0) * _startRotation;
        Vector3 targetPosition = playerTransform.position + offset.x * _pivotTransform.right +
                                 offset.y * Vector3.up + offset.z * _pivotTransform.forward;
        _pivotTransform.position =
            Vector3.Lerp(_pivotTransform.position, targetPosition, cameraChaseSpeed * Time.deltaTime);

        RaycastHit hit;
        Ray ray = new Ray(_pivotTransform.position, -_cameraTransform.forward);
        bool obstructed = Physics.SphereCast(ray, sphereCheckRadius, out hit, maxDistance, layerMask);

        if (obstructed && hit.distance < minDistance)
        {
            _pitch = Mathf.Clamp(_pitch, 30, 74);
            _pivotTransform.rotation = Quaternion.Euler(_pitch, _yaw, 0) * _startRotation;
        }

        Debug.DrawLine(_pivotTransform.position,
            _pivotTransform.position + _cameraTransform.forward * -(hit.distance - sphereCheckRadius), Color.red);
        _cameraTransform.localPosition =
            new Vector3(0, 0, obstructed ? -(hit.distance - sphereCheckRadius) : -maxDistance);
    }
}
