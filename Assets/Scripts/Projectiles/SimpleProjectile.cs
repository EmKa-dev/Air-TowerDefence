using AirTowerDefence.Common;
using UnityEngine;

namespace AirTowerDefence.Projectile
{
    public class SimpleProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private ParticleSystem HitEffect;

        private float _Speed;

        private int _Damage;

        private Transform Target;

        public void Initialize(Transform target, int damage, float speed)
        {
            Target = target;
            _Damage = damage;
            _Speed = speed;
        }
        private void Update()
        {
            if (Target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = Target.position - transform.position;

            float step = _Speed * Time.deltaTime;

            if (dir.magnitude > step)
            {
                dir = Target.position - transform.position;
                step = _Speed * Time.deltaTime;

                transform.Translate(dir.normalized * step, Space.World);

                return;
            }

            HitTarget();
        }

        private void HitTarget()
        {
            Target.GetComponent<IDamagable>().TakeDamage(_Damage);

            Destroy(this.gameObject);

        }
    }
}
