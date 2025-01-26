using System;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    private float speedBoostAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedBoostAmount = 200f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();
            if (playerController != null)
            {
                playerController.ApplySpeedBoost(speedBoostAmount);
            }
        }
    }
}
