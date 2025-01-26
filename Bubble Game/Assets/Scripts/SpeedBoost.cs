using System;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public enum BoostDirection
    {
        CameraForward,
        Upward
    }
    public BoostDirection boostDirection;
    public float speedBoostAmount = 200f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();
            if (playerController != null)
            {
                if(boostDirection == BoostDirection.CameraForward)
                    playerController.ApplySpeedBoost(speedBoostAmount);
                else if(boostDirection == BoostDirection.Upward)
                    other.GetComponent<Rigidbody>().AddForce(Vector3.up * speedBoostAmount, ForceMode.Impulse);
            }
        }
    }
}
