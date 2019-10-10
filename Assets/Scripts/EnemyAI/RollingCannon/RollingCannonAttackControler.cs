using AirTowerDefence.Projectile;
using System;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    [RequireComponent(typeof(PlayerDetector))]
    public class RollingCannonAttackControler : AttackController
    {

        private Animator _Animator;

        [SerializeField]
        private GameObject _ProjectilePrefab;

        [SerializeField]
        private Transform _RotationPivotPoint;

        [SerializeField]
        private float _RotationSpeed;

        private float _AttackTimer;

        private Transform _Target;


        private void Start()
        {
            _Animator = transform.root.GetComponentInChildren<Animator>();

            var d = GetComponent<PlayerDetector>();
            d.TargetDetected += TargetDetected;
            d.TargetLost += TargetLost;

            _AttackTimer = 1 / _AttackRate;
        }

        public override void UpdateControl()
        {
            _AttackTimer -= Time.deltaTime;

            if (_Target != null)
            {
                RotateTowardsTarget();

                AdjustAiming();

                if (_AttackTimer <= 0)
                {
                    Fire();

                    _AttackTimer = 1 / _AttackRate;
                }
            }
        }

        private void Fire()
        {
            var p = Instantiate(_ProjectilePrefab, transform.position, Quaternion.identity);
            p.GetComponent<IProjectile>().Initialize(_Target, 10, 5);

            _Animator.SetTrigger("OnLaunch");
        }

        private void AdjustAiming()
        {
            //Calculate angle we need to aim
            
            float angle = Vector3.Angle(transform.position, _Target.position);
            angle = 90 - angle;
            _Animator.SetFloat("AimingBlend", Mathf.Clamp(angle, 0, 90));

        }

        private void RotateTowardsTarget()
        {

            Vector3 dir = _Target.position - transform.position;

            Quaternion lookrot = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Slerp(transform.rotation, lookrot, Time.deltaTime * _RotationSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }

        private void TargetDetected(Transform target)
        {
            InvokeRequestControl();

            _Target = target;
        }

        private void TargetLost(Transform target)
        {
            _Target = null;
        }
    }
}
