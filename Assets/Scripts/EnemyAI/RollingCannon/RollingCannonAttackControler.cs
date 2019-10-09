using AirTowerDefence.Projectile;
using System;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    [RequireComponent(typeof(PlayerDetector))]
    public class RollingCannonAttackControler : AttackController
    {
        [SerializeField]
        private GameObject _ProjectilePrefab;

        private Animator _Animator;

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


        }

        private void RotateTowardsTarget()
        {
            Vector3 dir = _Target.position - transform.position;

            Quaternion lookrot = Quaternion.LookRotation(dir);
            transform.rotation = lookrot;
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
