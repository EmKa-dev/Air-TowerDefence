using System;
using UnityEngine;

namespace AirTowerDefence.Common
{
    public class Healthpool : MonoBehaviour, IDamagable
    {

        public event Action<(int lives, int health)> _HealthStatusChangedEvent;

        [SerializeField]
        private int _Lives;
        public int Lives
        {
            get { return _Lives; }
            private set
            { 
                _Lives = value;
                OnHealthStatusChanged();
            }
        }


        [SerializeField]
        private int _Health;
        public int Health
        {
            get { return _Health; }
            private set
            {
                _Health = value;
                OnHealthStatusChanged();
            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnHealthStatusChanged()
        {
            _HealthStatusChangedEvent?.Invoke((_Lives, _Health));
        }
    }
}
