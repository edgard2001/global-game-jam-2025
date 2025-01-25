using System;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform _defaultRespawnPoint;
    [SerializeField] private Player _player;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private bool _respawnPointActive = false;
    
    
    void Start()
    {
        _respawnPoint = _defaultRespawnPoint;
    }
    
    void Update()
    {
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player._respawnPoint = _respawnPoint;
            _respawnPointActive = true;
        }
    }
}
