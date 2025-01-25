using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    public Transform player; 
    public float forceStrength = 20f; 
    public float detectionRadius = 15f; 
    public float stopDistance = 1.5f; 

    private Rigidbody rb;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

           
            if (distanceToPlayer < detectionRadius && distanceToPlayer > stopDistance)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * forceStrength);
            }
            else if (distanceToPlayer <= stopDistance)
            {
                
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
