using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    public Transform player;
    public float forceStrength = 20f; 
    public float detectionRadius = 15f; 
    public float stopDistance = 1.5f; 
    public float chargeForceMultiplier = 50f; 
    public float chargeCooldown = 5f; 

    private Rigidbody rb;
    private bool isCharging = false;
    private Vector3 chargeDirection; 
    private float chargeTimer = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (isCharging)
            {
                
                return;
            }

            chargeTimer += Time.fixedDeltaTime;

            if (chargeTimer >= chargeCooldown && distanceToPlayer < detectionRadius)
            {
                StartCharge();
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

    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = 0f;
        
        chargeDirection = (player.position - transform.position).normalized;
        
        rb.AddForce(chargeDirection * chargeForceMultiplier, ForceMode.Impulse);

        
        Invoke(nameof(EndCharge), 1f); 
    }

    private void EndCharge()
    {
        isCharging = false;
    }
}