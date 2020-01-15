using UnityEngine;
using AirTowerDefence.EnemySpawn;

namespace AirTowerDefence.Enemy.Controllers
{

    public class MoveController : Controller, IMovingCreep
    {
        [SerializeField]
        private float _Speed;

        [SerializeField]
        private float _RotationSpeed;

        private Waypoint TargetWayPoint;
        private Vector3 _RelativePositionToWayPoint;

        private Vector3 _TargetPositionInWorldSpace;

        private bool IsRotatedTowardTarget;

        public void Initialize(Waypoint spawnpoint)
        {
            TargetWayPoint = spawnpoint;
            _TargetPositionInWorldSpace = transform.position;
            _RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
        }

        private void Awake()
        {
            //In case where this wasn't initialized by a spawnpoint, set target to some endpoint
            var end = GameObject.FindGameObjectWithTag("Endpoint");

            TargetWayPoint = end.GetComponent<Waypoint>();
            _TargetPositionInWorldSpace = end.transform.position;
        }

        public override void UpdateControl()
        {
            MoveTowardTargetPosition();
        }

        private void MoveTowardTargetPosition()
        {
            if (Vector3.Distance(transform.position, _TargetPositionInWorldSpace) < 0.01f)
            {
                GetNextTargetPosition();
                IsRotatedTowardTarget = false;
            }


            if (IsRotatedTowardTarget)
            {
                Vector3 targetvector = Vector3.MoveTowards(transform.position, _TargetPositionInWorldSpace, _Speed * Time.deltaTime);

                transform.position = targetvector;
            }
            else
            {
                RotateTowardsTarget();
            }
        }

        private void RotateTowardsTarget()
        {
            Vector3 dir = _TargetPositionInWorldSpace - transform.position;

            if (dir == Vector3.zero)
            {
                return;
            }

            Quaternion lookrot = Quaternion.LookRotation(dir);

            RotateBody();

            IsRotatedTowardTarget = IsBodyRotationComplete();

            void RotateBody()
            {
                Vector3 rotation = Quaternion.RotateTowards(transform.rotation, lookrot, _RotationSpeed * Time.deltaTime).eulerAngles;
                transform.rotation = Quaternion.Euler(0, rotation.y, 0);
            }

            bool IsBodyRotationComplete()
            {
                return Mathf.Abs(transform.rotation.eulerAngles.y - lookrot.eulerAngles.y) < 0.01f;
            }
        }

        private void GetNextTargetPosition()
        {
            if (TargetWayPoint.Next != null)
            {
                TargetWayPoint = TargetWayPoint.Next;
            }

            _TargetPositionInWorldSpace = TargetWayPoint.transform.TransformPoint(_RelativePositionToWayPoint);
        }
    }
}
