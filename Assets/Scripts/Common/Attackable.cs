using UnityEngine;

namespace AirTowerDefence.Common
{
    public class Attackable : MonoBehaviour, IDamagable
    {
        [SerializeField]
        private float _Health;

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
