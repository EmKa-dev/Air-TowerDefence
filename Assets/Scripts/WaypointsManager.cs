using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField]
    private LineRenderer _LineRendererPrefab;
    [SerializeField]
    private GameObject _WayPointPrefab;
    [SerializeField]
    private GameObject _StartPointPrefab;

    public List<Waypoint> Waypoints = new List<Waypoint>();
    
    public void CreateNewWaypoint()
    {
        GameObject obj;

        //If its the first, set different color
        if (Waypoints.Count < 1)
        {
            obj = Instantiate(_StartPointPrefab, transform.position, _WayPointPrefab.transform.rotation);
            obj.transform.SetParent(transform, true);
        }
        else
        {
            obj = Instantiate(_WayPointPrefab, transform.position, _WayPointPrefab.transform.rotation);
            obj.transform.SetParent(transform, true);
        }


        AddNewWaypointAndCreateLink(obj.GetComponent<Waypoint>());

        obj.name = $"Waypoint {Waypoints.Count}";
        Selection.activeGameObject = obj;
    }

    private void AddNewWaypointAndCreateLink(Waypoint latestwaypoint)
    {

        if (Waypoints.Count > 0)
        {
            Waypoints.Last().Next = latestwaypoint;
        }

        Waypoints.Add(latestwaypoint);
    }

    private void GetCamerasCenter()
    {

    }

    private void OnDrawGizmos()
    {
        RemoveDeletedWaypoints();

        if (Waypoints.Count > 1)
        {
            DrawPathsRecursively(Waypoints.First());
        }
    }

    private void RemoveDeletedWaypoints()
    {
        Waypoints.RemoveAll((x) => x == null);
    }

    private void DrawPathsRecursively(Waypoint rootpoint)
    {
        if (rootpoint == null || rootpoint.Next == null)
        {
            return;
        }

        //See if there's a linerenderer available already, otherwise create one
        LineRenderer pathrenderer = rootpoint.transform.GetComponentInChildren<LineRenderer>(true);

        if (pathrenderer == null)
        {
            pathrenderer = Instantiate(_LineRendererPrefab);
            pathrenderer.transform.SetParent(rootpoint.transform, true);
        }

        DrawPathBetweenPoints(rootpoint.transform.position, rootpoint.Next.transform.position, pathrenderer);

        DrawPathsRecursively(rootpoint.Next);
    }

    private void DrawPathBetweenPoints(Vector3 point1, Vector3 point2, LineRenderer linerenderer)
    {

        List<Vector3> segments = new List<Vector3>();
        const float segmentlength = 1f;

        Vector3 currentpoint = point1;

        while (Vector3.Distance(point1, point2) > segmentlength/2)
        {
            var n = Vector3.MoveTowards(currentpoint, point2, segmentlength);
            currentpoint = n;

            segments.Add(n);

            //Only here as some kind of failsafe
            if (segments.Count > 100)
            {
                break;
            }
        }

        linerenderer.positionCount = segments.Count;
        linerenderer.SetPositions(segments.ToArray());
    }
}
