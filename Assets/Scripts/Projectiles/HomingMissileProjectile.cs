using AirTowerDefence.Common;
using UnityEngine;
namespace AirTowerDefence.Projectile
{
    public class HomingMissileProjectile : MonoBehaviour, IProjectile
    {

        [SerializeField]
        private ParticleSystem _HitEffect;

        [SerializeField]
        private GameObject _TargetMarkerPrefab;

        private Transform _Target;

        private float _Damage;

        private float _Speed;

        private Vector3 _LastKnownTargetPosition;

        private GameObject _MarkerObject;

        public void Initialize(Transform target, float damage, float speed)
        {
            _Target = target;
            _Damage = damage;
            _Speed = speed;
            _LastKnownTargetPosition = target.position;
            CreateMarkerAtTarget();
        }

        public void Initialize(Vector3 lastknownposition, float damage, float speed)
        {
            _Damage = damage;
            _Speed = speed;
            _LastKnownTargetPosition = lastknownposition;
            CreateMarkerAtTarget();
        }

        void Update()
        {

            if (_Target != null)
            {
                _LastKnownTargetPosition = _Target.position;
            }

            TrackTarget();

        }
        private void CreateMarkerAtTarget()
        {
            _MarkerObject = Instantiate(_TargetMarkerPrefab, _LastKnownTargetPosition, _TargetMarkerPrefab.transform.rotation);
        }

        private void TrackTarget()
        {
            Vector3 dir = _LastKnownTargetPosition - transform.position;
            float step = _Speed * Time.deltaTime;

            if (dir.magnitude > step)
            {
                dir = _LastKnownTargetPosition - transform.position;
                //Rotate bullet
                Quaternion lookrot = Quaternion.LookRotation(dir);
                transform.rotation = lookrot;
                transform.Rotate(90, 0, 0);


                transform.Translate(dir.normalized * step, Space.World);
                _MarkerObject.transform.position = _LastKnownTargetPosition + Vector3.up * 0.1f;

                return;
            }

            HitTarget();
        }

        private void Explode()
        {
            if (_HitEffect != null)
            {
                var effect = Instantiate(_HitEffect, _LastKnownTargetPosition + (Vector3.up * 0.5f), Quaternion.identity);
                effect.Play();
            }

            Destroy(gameObject);
            Destroy(_MarkerObject);
        }

        private void HitTarget()
        {
            if (_Target != null)
            {
                _Target.GetComponent<IDamagable>().TakeDamage(_Damage);
            }

            Explode();
        }
    }
}
