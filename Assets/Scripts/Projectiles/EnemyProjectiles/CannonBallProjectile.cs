using AirTowerDefence.Common;
using UnityEngine;

namespace AirTowerDefence.Projectile
{
    public class CannonBallProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        private ParticleSystem _HitEffect;

        private float _Speed;

        private int _Damage;

        private Renderer playerRenderer;

        public void Initialize(Transform target, int damage, float speed)
        {
            playerRenderer = target.GetComponent<Renderer>();

            _Damage = damage;
            _Speed = speed;
        }

        void Update()
        {

            if (playerRenderer == null)
            {
                return;
            }

            transform.position += transform.forward * _Speed * Time.deltaTime;

            if (IsMakingContactWithTarget())
            {
                HitTarget();
            }
        }

        private bool IsMakingContactWithTarget()
        {
            if (playerRenderer.bounds.Contains(this.transform.position))
            {
                return true;
            }

            return false;
        }

        private void HitTarget()
        {
            playerRenderer.GetComponent<IDamagable>().TakeDamage(_Damage);

            if (_HitEffect != null)
            {
                var effect = Instantiate(_HitEffect, this.transform.position, Quaternion.identity);
                effect.Play();
            }

            Destroy(this.gameObject);
        }
    }
}
