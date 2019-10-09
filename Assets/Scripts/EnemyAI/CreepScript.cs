using AirTowerDefence.Common;
using AirTowerDefence.EnemySpawn;
using UnityEngine;

namespace AirTowerDefence.Enemy
{
    public class CreepScript : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private float _Health;

        [SerializeField]
        private float _Speed;

        private SquareWaypoint TargetWayPoint;

        private Animator _Animator;

        private Vector3 TargetPositionInWorldSpace;

        private Vector3 RelativePositionToWayPoint;


        private float aim;

        void Start()
        {
            _Animator = transform.root.GetComponentInChildren<Animator>();
            FindSpawnPoint();
            TargetPositionInWorldSpace = transform.position;
            RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
        }

        void Update()
        {
            MoveTowardTargetWayPoint();

            //Testing code
            if (_Animator != null)
            {
                aim = Mathf.PingPong(Time.time * 10, 90);
                _Animator.SetFloat("AimingBlend", aim);
            }
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

        public void TakeDamage(float damage)
        {
            this._Health -= damage;

            if (_Health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
