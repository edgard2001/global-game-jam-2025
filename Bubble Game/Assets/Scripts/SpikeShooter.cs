using System;
using UnityEngine;

public class SpikeShooter : MonoBehaviour
{
    public GameObject spikePrefab;
    public Transform spawnPoint;
    public float spikeSpeed = 10f;
    public float spikeLifetime = 5f;
    public bool continuousShooting = false;
    public float shootDelay = 1f;
    
    private float shootTimer;
    private AudioSource audio;
    public AudioClip shootSound;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        
        if (continuousShooting)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootDelay)
            {
                ShootSingleSpike(); 
                shootTimer = 0f;
            }
        }
    }

 

    private void ShootSingleSpike()
    {
        Quaternion rotation =  Quaternion.LookRotation(transform.forward);
        GameObject spike = Instantiate(spikePrefab, spawnPoint.position,rotation);
        Rigidbody rb = spike.GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * spikeSpeed;
        Destroy(spike, spikeLifetime);
        audio.PlayOneShot(shootSound);
    }
    
}