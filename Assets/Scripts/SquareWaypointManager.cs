using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SquareWaypointManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _WayPointPrefab;
    [SerializeField]
    private GameObject _StartPointPrefab;

    public List<SquareWaypoint> Waypoints;

    public void CreateNewWaypoint()
    {
        GameObject obj;

        //If its the first, set different color
        if (Waypoints.Count < 1)
        {
            obj = Instantiate(_StartPointPrefab, _WayPointPrefab.transform.position, _WayPointPrefab.transform.rotation);
            obj.transform.SetParent(transform, true);
        }
        else
        {
            obj = Instantiate(_WayPointPrefab, Waypoints.First().transform.position, _WayPointPrefab.transform.rotation);
            obj.transform.SetParent(transform, true);
        }


        AddNewWaypointAndCreateLink(obj.GetComponent<SquareWaypoint>());

        obj.name = $"Waypoint {Waypoints.Count}";
        Selection.activeGameObject = obj;
    }

    private void AddNewWaypointAndCreateLink(SquareWaypoint latestwaypoint)
    {
        //Set link
        if (Waypoints.Count > 0)
        {
            Waypoints.Last().Next = latestwaypoint;
        }

        Waypoints.Add(latestwaypoint);
    }

    private void OnDrawGizmos()
    {
        if (Waypoints.Count > 1)
        {
            DrawPathsRecursively(Waypoints.First());
        }
    }


    private void DrawPathsRecursively(SquareWaypoint rootpoint)
    {
        if (rootpoint == null || rootpoint.Next == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rootpoint.transform.position, rootpoint.Next.transform.position);

        DrawPathsRecursively(rootpoint.Next);
    }
}
