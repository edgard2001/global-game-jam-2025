using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject SparkleFX;
    public GameObject CoinCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SparkleFX.SetActive(false);
        
        // Find child object with the name "center"
        CoinCenter = transform.Find("center").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // rotate the coin
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Add coin to player
            // other.GetComponent<Player>().AddCoin();
            Debug.Log("Player has collected a coin");

            SparkleFX.SetActive(true);
            CoinCenter.SetActive(false);
            
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            
            // Destroy coin
            StartCoroutine(DestroyCoin());
        }
    }
    
    
    // IEnumerator to destroy coin after 2 seconds
    private IEnumerator DestroyCoin()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2);
        // Destroy coin
        Destroy(gameObject);
    }
    
    
}
