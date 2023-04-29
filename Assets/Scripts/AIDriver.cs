using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDriver : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] public Transform[] customDestinations;
    [SerializeField] private float carSpeed = 10f;
    [SerializeField] private float customDestinationWaitTime = 5f;

    private Vector3 currentWaypoint;
    private int currentWaypointIndex = 0;
    private bool waitingAtCustomDestination = false;
    private Queue<int> customDestinationQueue = new Queue<int>();

    private void Start()
    {
        navMeshAgent.speed = carSpeed;

        // Find nearest waypoint to start from
        currentWaypoint = FindNearestWaypoint();

        navMeshAgent.SetDestination(currentWaypoint);
    }

    private void Update()
    {
        // If car is waiting at a custom destination, don't do anything
        if (waitingAtCustomDestination)
        {
            return;
        }

        // If car has reached current waypoint, go to next one
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // If there are custom destinations queued up, go to the next one instead
            if (customDestinationQueue.Count > 0)
            {
                int nextCustomDestinationIndex = customDestinationQueue.Dequeue();
                GoToCustomDestination(nextCustomDestinationIndex);
                return;
            }

            // Otherwise, go to the next waypoint
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }

            currentWaypoint = waypoints[currentWaypointIndex].position;
            navMeshAgent.SetDestination(currentWaypoint);
        }
    }

    private Vector3 FindNearestWaypoint()
    {
        Vector3 nearestWaypoint = waypoints[0].position;
        float shortestDistance = Mathf.Infinity;

        // Find the nearest waypoint
        foreach (var waypoint in waypoints)
        {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestWaypoint = waypoint.position;
            }
        }

        return nearestWaypoint;
    }

    public void GoToCustomDestination(int index)
    {
        if (index >= 0 && index < customDestinations.Length)
        {
            navMeshAgent.SetDestination(customDestinations[index].position);
            waitingAtCustomDestination = true;
            StartCoroutine(WaitAtCustomDestination());
        }
    }

    private IEnumerator WaitAtCustomDestination()
    {
        yield return new WaitForSeconds(customDestinationWaitTime);
        waitingAtCustomDestination = false;
        currentWaypointIndex = 0;
        currentWaypoint = FindNearestWaypoint();
        navMeshAgent.SetDestination(currentWaypoint);
    }

    public void AddCustomDestination(int index)
    {
        if (index >= 0 && index < customDestinations.Length)
        {
            customDestinationQueue.Enqueue(index);
        }
    }

    public void ClearCustomDestinations()
    {
        customDestinationQueue.Clear();
    }

    public void MoveToCenter()
    {
        // Get the center of the NavMeshSurface
        Vector3 center = NavMesh.GetSettingsByID(0).agentTypeID == 0 ? navMeshAgent.gameObject.GetComponent<BoxCollider>().center : Vector3.zero;

        // Move the car to the center of the NavMeshSurface
        navMeshAgent.Warp(center);
    }
}
