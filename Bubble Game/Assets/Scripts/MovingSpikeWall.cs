using UnityEngine;

public class MovingSpikeWall : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Select the movement direction.")]
    public MovementDirection movementDirection = MovementDirection.UpAndDown;

    [Tooltip("The distance the object will move.")]
    public float moveDistance = 5f;

    [Tooltip("The speed of the movement.")]
    public float moveSpeed = 2f;

    [Tooltip("Delay in seconds before reversing direction.")]
    public float delayBeforeReverse = 0f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingForward = true;
    private float delayTimer = 0f;
    private bool isDelaying = false;

    public enum MovementDirection
    {
        UpAndDown,
        LeftAndRight,
        BackAndForth,
        ForwardBackward,
        RightLeft // New options
    }

    void Start()
    {
        startPos = transform.position;

        switch (movementDirection)
        {
            case MovementDirection.UpAndDown:
                targetPos = startPos + new Vector3(0, moveDistance, 0);
                break;
            case MovementDirection.LeftAndRight:
                targetPos = startPos + new Vector3(moveDistance, 0, 0);
                break;
            case MovementDirection.BackAndForth:
                targetPos = startPos + new Vector3(0, 0, moveDistance);
                break;
            case MovementDirection.ForwardBackward:
                targetPos = startPos + new Vector3(0, 0, moveDistance);
                break;
            case MovementDirection.RightLeft:
                targetPos = startPos + new Vector3(moveDistance, 0, 0);
                break;
        }
    }

    void Update()
    {
        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayBeforeReverse)
            {
                isDelaying = false;
                delayTimer = 0f;
            }
            return;
        }

        MoveSpike();
    }

    private void MoveSpike()
    {
        Vector3 target = movingForward ? targetPos : startPos;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            movingForward = !movingForward; // Reverse direction
            if (delayBeforeReverse > 0f)
            {
                isDelaying = true; // Start delay
            }
        }
    }
}