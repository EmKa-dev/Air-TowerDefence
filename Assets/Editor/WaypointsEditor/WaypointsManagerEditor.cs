using AirTowerDefence.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AirTowerDefence.EditorTool
{
    [CustomEditor(typeof(WaypointManager))]
    public class WaypointsManagerEditor : Editor
    {
        private WaypointManager manager;

        private void OnEnable()
        {
            manager = (WaypointManager)target;
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

            if (GUILayout.Button("Convert last to endpoint"))
            {
                manager.ConvertLastToEndpoint();
            }
        }

        private void OnSceneGUI()
        {
            manager.CheckAndRemoveDeletedWaypoints();
        }
    }
}
