using AirTowerDefence.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AirTowerDefence.EditorTool
{
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
                Selection.activeGameObject = manager.Waypoints.Last().gameObject;
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
}
