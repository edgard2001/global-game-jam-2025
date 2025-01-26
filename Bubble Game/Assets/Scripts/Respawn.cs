using System;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    
    [SerializeField] private Player _player;
    [SerializeField] private Transform _respawnPoint;
    


    private void Start()
    {
        _player = FindAnyObjectByType<Player>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player._respawnPoint = _respawnPoint;
            
        }
    }
}
