using UnityEngine;

public class SpikeShooter : MonoBehaviour
{
    public GameObject spikePrefab;
    public Transform spawnPoint;
    public bool continuousShooting = false;
    public float shootDelay = 1f;
    public float spikeSpeed = 10f;
    public float spikeLifetime = 5f;
    public bool useAngle = false;
    public float angleRange = 180f;
    public int spikesPerShot = 1;

    private float shootTimer;

    void Update()
    {
        if (continuousShooting)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootDelay)
            {
                ShootSpikes();
                shootTimer = 0f;
            }
        }
    }

    public void ShootSpikes()
    {
        if (useAngle && spikesPerShot > 1)
        {
            ShootSpikesInAngle();
        }
        else
        {
            ShootSingleSpike();
        }
    }

    private void ShootSingleSpike()
    {
        Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0, 90f, 0f);
        GameObject spike = Instantiate(spikePrefab, spawnPoint.position, rotation);
        Rigidbody rb = spike.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.right * spikeSpeed;
        }
        Destroy(spike, spikeLifetime);
    }

    private void ShootSpikesInAngle()
    {
        float angleStep = angleRange / (spikesPerShot - 1);
        float startAngle = -angleRange / 2;

        for (int i = 0; i < spikesPerShot; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0, 0f, 90f) * Quaternion.Euler(0, currentAngle, 0f);
            GameObject spike = Instantiate(spikePrefab, spawnPoint.position, rotation);
            Rigidbody rb = spike.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = rotation * Vector3.right * spikeSpeed;
            }
            Destroy(spike, spikeLifetime);
        }
    }
}