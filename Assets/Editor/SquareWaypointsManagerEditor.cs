using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SquareWaypointManager))]
public class SquareWaypointsManagerEditor : Editor
{
    SquareWaypointManager manager;

    private void OnEnable()
    {
        manager = (SquareWaypointManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string buttontext = (manager.Waypoints.Count == 0) ? "Create spawnpoint" : "Create waypoint";

        if (GUILayout.Button(buttontext))
        {
            manager.CreateNewWaypoint();
        }
    }

    private void OnSceneGUI()
    {
        if (manager.Waypoints.Any(x => x == null))
        {
            RemoveDeletedWaypoints();
        }
    }

    private void RemoveDeletedWaypoints()
    {
        manager.Waypoints.RemoveAll((x) => x == null);
    }
}
