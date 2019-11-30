using AirTowerDefence.EnemySpawn;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO

// If multiple endpoints, convert previous endpoints back to normal waypoints

namespace AirTowerDefence.Managers
{
    public class WaypointManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _WayPointPrefab;
        [SerializeField]
        private GameObject _StartPointPrefab;
        [SerializeField]
        private GameObject _EndPointPrefab;

        public List<Waypoint> Waypoints;

        #region Editor

        public void CreateNewWaypoint()
        {
            GameObject obj;

            //If its the first, set different prefab
            if (Waypoints.Count < 1)
            {
                obj = Instantiate(_StartPointPrefab, _WayPointPrefab.transform.position, _WayPointPrefab.transform.rotation);
            }
            else
            {
                obj = Instantiate(_WayPointPrefab, Waypoints.First().transform.position, _WayPointPrefab.transform.rotation);
            }

            AddWaypoint(obj.GetComponent<Waypoint>());
        }

        public void ConvertLastToEndpoint()
        {
            if (Waypoints.Count < 2)
            {
                Debug.Log("Can not convert spawnpoint to endpoint");
                return;
            }

            var lastwaypoint = Waypoints.Last();

            var endpoint = Instantiate(_EndPointPrefab, lastwaypoint.transform.position, lastwaypoint.transform.rotation);

            Waypoints.Remove(lastwaypoint);
            DestroyImmediate(lastwaypoint.gameObject);

            AddWaypoint(endpoint.GetComponent<Waypoint>());
        }

        public void CheckAndRemoveDeletedWaypoints()
        {
            if (Waypoints.Any(x => x == null))
            {
                Waypoints.RemoveAll((x) => x == null);

                RenameWayPointsInOrder();
                CreateLinksBetweenAllWaypoints();
            }
        }

        private void AddWaypoint(Waypoint newwaypoint)
        {
            newwaypoint.transform.SetParent(transform, true);

            Waypoints.Add(newwaypoint);

            RenameWayPointsInOrder();
            CreateLinksBetweenAllWaypoints();
        }

        private void RenameWayPointsInOrder()
        {
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Waypoints[i].name = $"Waypoint {i.ToString()}";
            }
        }

        private void CreateLinksBetweenAllWaypoints()
        {
            if (Waypoints.Count < 1)
            {
                return;
            }

            for (int i = 0; i < Waypoints.Count; i++)
            {
                Waypoints[i].Next = i == Waypoints.Count - 1 ? null : Waypoints[i + 1];
            }
        }

        private void DrawPathsRecursively(Waypoint rootpoint)
        {
            if (rootpoint == null || rootpoint.Next == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(rootpoint.transform.position, rootpoint.Next.transform.position);

            DrawPathsRecursively(rootpoint.Next);
        }

        private void OnDrawGizmos()
        {
            if (Waypoints != null && Waypoints.Count > 1)
            {
                DrawPathsRecursively(Waypoints.First());
            }
        }

        #endregion

        #region PlayMode

        public Waypoint GetSpawnPoint()
        {
            return Waypoints.Find(x => x.CompareTag("Spawnpoint"));
        }

        #endregion
    }
}
