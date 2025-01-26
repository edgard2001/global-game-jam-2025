using System;
using UnityEngine;

public class HurtPlayerWithHeatOrCold : MonoBehaviour
{
    public enum HurtType
    {
        Heat,
        Cold
    }
    
    public HurtType selectedHurtType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Damage depending on heat or cold
            if (selectedHurtType == HurtType.Heat)
            {
                Debug.Log("Player has entered the heat trigger");
                // Player will take expand
                // other.GetComponent<PlayerHealth>().ExpandPlayer();
                other.GetComponent<Player>().ShouldGrow();
            }
            else if (selectedHurtType == HurtType.Cold)
            {
                Debug.Log("Player has entered the cold trigger");
                // Player will take shrink
                other.GetComponent<Player>().ShouldShrink();
            }
            Debug.Log("Player will die");
            // Player will bounce backwards
            // other.GetComponent<Rigidbody>().AddForce(-other.transform.forward * 100000f);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Player>().EndResizeEffect();
        }
    }
}
