using System.Collections;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    public int health = 3;
    public Transform player;
    public float forceStrength = 20f;
    public float detectionRadius = 15f;
    public float stopDistance = 1.5f;
    public float chargeForceMultiplier = 50f;
    public float chargeCooldown = 5f;
    public float chargePauseDuration = 1f; // Pause before charging
    public float overshootDistance = 2f;  // Roll past the player's last position

    private Rigidbody rb;
    private bool isCharging = false;
    private bool isPaused = false;
    private Vector3 chargeTargetPosition; // Player's last known position for the charge
    private float chargeTimer = 0f;

    private Vector3 _startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isCharging || isPaused || distanceToPlayer > detectionRadius)
                return;

            chargeTimer += Time.fixedDeltaTime;

            if (chargeTimer >= chargeCooldown)
            {
                StartCoroutine(WaitAndCharge());
            }
            else if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer(distanceToPlayer);
            }
            else if (distanceToPlayer <= stopDistance)
            {
                StopMovement();
            }
        }
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.AddForce(direction * forceStrength);
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private IEnumerator WaitAndCharge()
    {
        isPaused = true;
        chargeTargetPosition = player.position; // Lock the player's current position
        yield return new WaitForSeconds(chargePauseDuration);

        isPaused = false;
        StartCharge();
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0f;

        Vector3 direction = (chargeTargetPosition - transform.position).normalized;
        Vector3 overshoot = direction * overshootDistance; // Add overshoot
        Vector3 finalChargePosition = chargeTargetPosition + overshoot;

        rb.AddForce(direction * chargeForceMultiplier, ForceMode.Impulse);

        Invoke(nameof(EndCharge), 1f);
    }

    private void EndCharge()
    {
        isCharging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(Respawn), 2f);
        }
    }

    private void Respawn()
    {
        transform.position = _startPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        chargeTimer = 0f;
        isCharging = false;
        isPaused = false;
    }
}