using UnityEngine;
using UnityEngine.AI;

public class NPCRandomMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float turnSpeed = 5f;
    public float walkingSpeed = 1.5f;
    public float runSpeed = 4f;
    public float waypointTolerance = 1f;
    public LayerMask obstacleMask;

    private int currentWaypoint = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private bool isWalking = false;
    private float turnDirection = 0f;
    private float currentSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("Turn", turnDirection);
        GoToNextWaypoint();
    }

    void Update()
    {
        RaycastHit obstacleHit;
        if (HasObstacleInFront(out obstacleHit))
        {
            // Set a new target position to the left or right of the obstacle
            Vector3 offset = Vector3.Cross(Vector3.up, obstacleHit.normal);
            Vector3 newTarget = transform.position + offset * 2f; // adjust offset as needed
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetPath(path);
                isWalking = true;
            }
            else
            {
                // Pathfinding failed, stop moving
                agent.velocity = Vector3.zero;
                isWalking = false;
            }
        }
        else if (!agent.pathPending && agent.remainingDistance < waypointTolerance)
        {
            GoToNextWaypoint();
        }

        if (agent.velocity.normalized != Vector3.zero)
        {
            isWalking = true;
            animator.SetBool("isWalking", isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
        }

        turnDirection = Vector3.Cross(transform.forward, target.position - transform.position).y;
        animator.SetFloat("Turn", turnDirection);

        if (turnDirection < -0.1f || turnDirection > 0.1f)
        {
            currentSpeed = turnSpeed;
        }
        else
        {
            currentSpeed = isWalking ? walkingSpeed : runSpeed;
        }

        agent.speed = currentSpeed;
        agent.SetDestination(target.position);
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        target = waypoints[currentWaypoint];
    }

    bool HasObstacleInFront(out RaycastHit hit)
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleMask))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }
}
