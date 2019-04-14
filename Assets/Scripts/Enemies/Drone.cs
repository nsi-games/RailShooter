using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Enemy
{
    public enum State
    {
        Patrol,
        Seek
    }

    public State currentState = State.Patrol;
    public float movementSpeed = 20f;
    public float rotationSpeed = 20f;
    public float distanceToTarget = 1f;
    public float detectionRadius = 5f;
    public Waypoints waypoints;
    
    // Waypoints
    private Transform target;
    private int currentWaypoint = 0;
    
    void OnDrawGizmosSelected()
    {
        waypoints.DrawWaypoints();
        // If the agent is in Patrol
        if (currentState == State.Patrol)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

    private void Start()
    {
        target = waypoints.GetPoint(currentWaypoint);
    }

    #region States
    void Patrol()
    {
        // Get distance to waypoint
        float distance = Vector3.Distance(transform.position, target.position);
        // If waypoint is within range
        if (distance <= distanceToTarget)
        {
            // Move to next waypoint (Next Frame)
            currentWaypoint = waypoints.ValidIndex(currentWaypoint + 1);
            target = waypoints.GetPoint(currentWaypoint);
        }
        else
        {
            Vector3 direction = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, movementSpeed * Time.deltaTime);
            // Generate path to current waypoint
            transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);
        }

        // Overlap sphere to detect things
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            Player player = hit.GetComponent<Player>();
            if (player)
            {
                target = player.transform;
                currentState = State.Seek;
            }
        }
    }
    void Seek()
    {
        // Update the AI's target position
        transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);
        // Get distance to target
        float distance = Vector3.Distance(transform.position, target.position);
        // If the target is outside detection range
        if (distance >= detectionRadius)
        {
            // Switch to patrol
            currentState = State.Patrol;
            // Get current waypoint
            target = waypoints.GetPoint(currentWaypoint);
        }
        
        if(distance >= distanceToTarget)
        {
            Vector3 direction = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, movementSpeed * Time.deltaTime);
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                Seek();
                break;
            default:
                break;
        }
    }
}
