using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public bool loop = false;
    public bool show = false;

    private Transform[] waypoints;

    #region Debugging
    private void OnDrawGizmos()
    {
        if (show)
        {
            waypoints = GetComponentsInChildren<Transform>();
            DrawWaypoints();
        }
    }

    public void DrawWaypoints()
    {
        if(waypoints == null)
            waypoints = GetComponentsInChildren<Transform>();
        // Set i to 1 to skip the parent
        for (int i = 1; i < waypoints.Length - 1; i++)
        {
            Transform start = waypoints[i];
            Transform end = waypoints[i + 1];
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start.position, end.position);
        }

        if (loop && waypoints.Length > 1)
        {
            Transform start = waypoints[1];
            Transform end = waypoints[waypoints.Length - 1];
            Gizmos.DrawLine(start.position, end.position);
        }
    }
    #endregion

    void Awake()
    {
        waypoints = GetComponentsInChildren<Transform>();
    }

    public Transform GetPoint(int index)
    {
        return waypoints[ValidIndex(index)];
    }

    public int ValidIndex(int index)
    {
        if (index >= waypoints.Length)
        {
            index = loop ? 1 : waypoints.Length - 1;
        }
        if (index < 1)
        {
            index = loop ? waypoints.Length - 1 : 1;
        }
        return index;
    }
}
