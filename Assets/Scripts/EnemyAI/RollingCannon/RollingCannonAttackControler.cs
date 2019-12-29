using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    [RequireComponent(typeof(PlayerDetector))]
    public class RollingCannonAttackControler : AttackController
    {
        [SerializeField]
        private Transform _Muzzle;

        [SerializeField]
        private GameObject _ProjectilePrefab;

        [SerializeField]
        private float _RotationSpeed;

        [SerializeField]
        private float _BarrelAdjustSpeed;

        [SerializeField]
        private float AfterFireStunTime;

        private Animator _Animator;

        private Transform _Target;

        private float _AttackTimer;

        private float _StunnedAfterFiringTimer;

        private float _CurrentBarrelAngle;

        private bool _ReadyToFire;

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

            if (_StunnedAfterFiringTimer > 0f)
            {
                return;
            }

            _AttackTimer -= Time.deltaTime;

            AdjustAiming();

            if (_ReadyToFire && _AttackTimer <= 0f)
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
            Quaternion lookrot = Quaternion.LookRotation(dir);

            RotateBody();

            if (IsBodyRotationNearlyComplete())
            {
                RotateBarrel();
            }
            else
            {
                _ReadyToFire = false;
            }

            void RotateBody()
            {
                Vector3 rotation = Quaternion.RotateTowards(transform.rotation, lookrot, _RotationSpeed * Time.deltaTime).eulerAngles;
                transform.rotation = Quaternion.Euler(0, rotation.y, 0);
            }

            bool IsBodyRotationNearlyComplete()
            {
                return Mathf.Abs(transform.rotation.eulerAngles.y - lookrot.eulerAngles.y) < 10f;
            }

            void RotateBarrel()
            {

                float angle = Vector3.Angle(transform.forward, dir);
                float smoothedangle = Mathf.MoveTowards(_CurrentBarrelAngle, angle, _BarrelAdjustSpeed * Time.deltaTime);

                _CurrentBarrelAngle = Mathf.Clamp(smoothedangle, 0, 90);

                _Animator.SetFloat("AimingBlend", _CurrentBarrelAngle);

                if (Mathf.Abs(angle - smoothedangle) < 1f)
                {
                    _ReadyToFire = true;
                }
            }
        }



        private void TargetDetected(Transform target)
        {
            _Target = target;
            InvokeRequestControl();
        }

        private void TargetLost(Transform target)
        {
            _Target = null;
            InvokeReliquishControl();
        }
    }
}
