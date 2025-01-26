using UnityEngine;

public class BallTriggerScript : MonoBehaviour
{
    public Transform objectToMove;
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    public bool startsOpen = false;

    public Renderer objectToChangeColor; // Reference to the object's renderer
    public Material defaultMaterial; // Original red material
    public Material triggeredMaterial; // New material to change to

    private float initialYPosition;
    private float targetPosY;

    private void Start()
    {
        initialYPosition = objectToMove.position.y;
        targetPosY = initialYPosition;
    }

    private void Update()
    {
        objectToMove.position = Vector3.Lerp(objectToMove.position, 
            new Vector3(objectToMove.position.x, targetPosY, objectToMove.position.z), 
            Time.deltaTime * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            targetPosY = initialYPosition - moveDistance;
            
            // Change material when triggered
            if (objectToChangeColor != null && triggeredMaterial != null)
            {
                objectToChangeColor.material = triggeredMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            targetPosY = initialYPosition;
            
            // Revert to default material
            if (objectToChangeColor != null && defaultMaterial != null)
            {
                objectToChangeColor.material = defaultMaterial;
            }
        }
    }
}