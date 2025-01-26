using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool startAtTop = false;
    public Vector3 initialPosition;
    public Vector3 finalPosition;
    public float speed = 1.0f;
    public float waitTime = 1.0f;

    private float timer = 0;
    
    public bool moveUp = true; // If true, the platform will move up . If false, the platform will move down.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        if (startAtTop)
        {
            moveUp = false;
            finalPosition = new Vector3(initialPosition.x, initialPosition.y - 5, initialPosition.z);
        }
        else
        {
            finalPosition = new Vector3(initialPosition.x, initialPosition.y + 5, initialPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= waitTime)
        {
            timer += Time.deltaTime;
            return;
        }
        // move platform
        if (moveUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
        }
        
        // check if platform has reached final position
        if (transform.position == finalPosition)
        {
            moveUp = false;
            timer = 0;
        }
        // check if platform has reached initial position
        if (transform.position == initialPosition)
        {
            moveUp = true;
            timer = 0;
        }
        
    }
    
}
