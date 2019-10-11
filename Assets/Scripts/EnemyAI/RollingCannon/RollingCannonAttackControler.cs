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
        private Transform _Muzzle;

        [SerializeField]
        private GameObject _ProjectilePrefab;

        [SerializeField]
        private float _RotationSpeed;

        [SerializeField]
        private float AfterFireStunTime;

        private Transform _Target;

        private float _AttackTimer;

        private float _StunnedAfterFiringTimer;


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
            if (_Target == null)
            {
                return;
            }

            _StunnedAfterFiringTimer -= Time.deltaTime;

            if (_StunnedAfterFiringTimer > 0)
            {
                return;
            }

            _AttackTimer -= Time.deltaTime;

            RotateTowardsTarget();

            AdjustAiming();

            if (_AttackTimer <= 0)
            {
                Fire();

                _AttackTimer = 1 / _AttackRate;
            }

        }

        private void Fire()
        {
            var p = Instantiate(_ProjectilePrefab, _Muzzle.position, _Muzzle.rotation);

            _Animator.SetTrigger("OnLaunch");
            _StunnedAfterFiringTimer = AfterFireStunTime;
        }

        private void AdjustAiming()
        {
            Vector3 dir = _Target.position - transform.position;

            float angle = Vector3.Angle(transform.forward, dir);
            _Animator.SetFloat("AimingBlend", Mathf.Clamp(angle, 0, 90) );

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
