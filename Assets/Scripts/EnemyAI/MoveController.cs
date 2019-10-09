using UnityEngine;
using AirTowerDefence.EnemySpawn;
using System;

namespace AirTowerDefence.Enemy.Controllers
{

    public class MoveController : Controller
    {
        [SerializeField]
        private float _Speed;

        private SquareWaypoint TargetWayPoint;

        private Vector3 TargetPositionInWorldSpace;

        private Vector3 RelativePositionToWayPoint;

        void Start()
        {
            FindSpawnPoint();
            TargetPositionInWorldSpace = transform.position;
            RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);

            InvokeRequestControl();
        }

        public override void UpdateControl()
        {
            MoveTowardTargetWayPoint();
        }

        private void FindSpawnPoint()
        {
            TargetWayPoint = GameObject.FindGameObjectWithTag("Spawnpoint").GetComponent<SquareWaypoint>();
        }

        private void MoveTowardTargetWayPoint()
        {
            if (Vector3.Distance(transform.position, TargetPositionInWorldSpace) < 0.001f)
            {
                GetNextTargetPosition();
                RotateTowardsTarget();
            }

            Vector3 targetvector = Vector3.MoveTowards(transform.position, TargetPositionInWorldSpace, _Speed * Time.deltaTime);

            transform.position = targetvector;
        }

        private void RotateTowardsTarget()
        {
            Vector3 dir = TargetWayPoint.transform.position - transform.position;

            Quaternion lookrot = Quaternion.LookRotation(dir);
            transform.rotation = lookrot;
        }

        private void GetNextTargetPosition()
        {
            if (TargetWayPoint.Next != null)
            {
                TargetWayPoint = TargetWayPoint.Next;
            }

            TargetPositionInWorldSpace = TargetWayPoint.transform.TransformPoint(RelativePositionToWayPoint);
        }
    }
}
