using UnityEngine;
using AirTowerDefence.EnemySpawn;

namespace AirTowerDefence.Enemy.Controllers
{

    public class MoveController : Controller, ISpawnableCreep
    {
        [SerializeField]
        private float _Speed;

        [SerializeField]
        private float _RotationSpeed;

        private Waypoint TargetWayPoint;
        private Vector3 _RelativePositionToWayPoint;

        private Vector3 _TargetPositionInWorldSpace;

        private bool IsRotatedTowardTarget;

        private Animator _Animator;

        public void Initialize(Waypoint spawnpoint)
        {
            TargetWayPoint = spawnpoint;
            _TargetPositionInWorldSpace = transform.position;
            _RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
        }

        private void Awake()
        {
            _Animator = transform.root.GetComponentInChildren<Animator>();

            //In case where this wasn't initialized by a spawnpoint, set target to some endpoint
            var end = GameObject.FindGameObjectWithTag("Endpoint");

            TargetWayPoint = end.GetComponent<Waypoint>();
            _TargetPositionInWorldSpace = end.transform.position;
        }

        public override void UpdateControl()
        {
            if (HasReachedTarget())
            {
                GetNextTargetPosition();
                IsRotatedTowardTarget = false;
            }

            if (IsRotatedTowardTarget)
            {
                if (_Animator != null)
                {
                    _Animator.SetBool("Moving", true);
                }

                MoveTowardTargetPosition();
            }
            else
            {
                RotateTowardsTarget();

                if (_Animator != null)
                {
                    _Animator.SetBool("Moving", false);
                }
            }
        }

        private void MoveTowardTargetPosition()
        {
            Vector3 targetvector = Vector3.MoveTowards(transform.position, _TargetPositionInWorldSpace, _Speed * Time.deltaTime);

            transform.position = targetvector;
        }

        private bool HasReachedTarget()
        {
            return Vector3.Distance(transform.position, _TargetPositionInWorldSpace) < 0.01f;
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
                return (Mathf.Abs(transform.rotation.eulerAngles.y - lookrot.eulerAngles.y) < 0.01f);
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

        public override void OnControlLost()
        {
            IsRotatedTowardTarget = false;

            if (_Animator != null)
            {
                _Animator.SetBool("Moving", false);
            }
        }
    }
}
