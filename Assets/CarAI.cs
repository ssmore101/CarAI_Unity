using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform[] customDestinations;
    private int currentWaypointIndex = 0;
    private int currentCustomDestinationIndex = 0;
    private NavMeshAgent navMeshAgent;
    private bool isWaiting = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetNextWaypointAsDestination();
    }

    void Update()
    {
        if (isWaiting)
        {
            return;
        }

        if (navMeshAgent.remainingDistance < 0.5f && !navMeshAgent.pathPending)
        {
            if (currentCustomDestinationIndex < customDestinations.Length)
            {
                StartCoroutine(WaitAndMoveToCustomDestination(5f));
            }
            else
            {
                SetNextWaypointAsDestination();
            }
        }
    }

    private void SetNextWaypointAsDestination()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public void GoToCustomDestination(int customDestinationIndex)
    {
        if (customDestinationIndex >= 0 && customDestinationIndex < customDestinations.Length)
        {
            currentCustomDestinationIndex = customDestinationIndex;
            navMeshAgent.SetDestination(customDestinations[currentCustomDestinationIndex].position);
        }
        else
        {
            SetNextWaypointAsDestination();
        }
    }

    IEnumerator WaitAndMoveToCustomDestination(float waitTime)
    {
        Debug.Log($"Reached custom destination at index {currentCustomDestinationIndex}");
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        currentCustomDestinationIndex++;
        GoToCustomDestination(currentCustomDestinationIndex);
    }
}
