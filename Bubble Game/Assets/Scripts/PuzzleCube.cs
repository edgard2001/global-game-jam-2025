using UnityEngine;

public class PuzzleCube : MonoBehaviour
{
    public GameObject Socket;
    public GameObject Player;
    public float allowableDistance = 5f;
    public float distanceFromPlayer = 5f;
    public float distanceFromSocket;
    
    private Vector3 initialPosition;
    // private Vector3 distanceFromSocket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Player");
        initialPosition = transform.position;
        distanceFromSocket = Vector3.Distance(initialPosition, Socket.transform.position);
        // distanceFromSocket = initialPosition - Socket.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition != initialPosition)
        {
            distanceFromPlayer = Vector3.Distance(currentPosition, Player.transform.position);
            distanceFromSocket = Vector3.Distance(currentPosition, Socket.transform.position);
            // Move the cube back to its initial position if it's too far away from the socket and player
            if (distanceFromSocket > allowableDistance && distanceFromPlayer > allowableDistance)
            {
                Debug.Log("Far from socket and player");
                transform.position = initialPosition;
            }
        }
    }
    
    private bool isFarFromObject(GameObject obj, Vector3 currentPosition)
    {
        return Vector3.Distance(currentPosition, obj.transform.position) > allowableDistance;
    }
}
