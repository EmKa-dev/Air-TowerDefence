using AirTowerDefence.Enemy.Controllers;
using System;
using UnityEngine;

namespace AirTowerDefence.Common
{
    public class Healthpool : MonoBehaviour, IDamagable
    {
        /// <summary>
        /// invoked once after initialization and for status changes.
        /// </summary>
        public event Action<(int lives, int health)> _HealthStatusChangedEvent;

        private int _MaxHealth;

        [SerializeField]
        private int _Lives;
        public int Lives
        {
            get { return _Lives; }
            private set
            { _Lives = value; }
        }

        [SerializeField]
        private int _Health;
        public int Health
        {
            get { return _Health; }
            private set
            {
                if (value > _MaxHealth) 
                { _Health = _MaxHealth; }
                else
                { _Health = value; }
            }
        }

        [SerializeField]
        private bool PlayAnimationOnDeath;
        [SerializeField]
        private GameObject ObjectToDestroy;

        private void Awake()
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _MaxHealth = 3;
            }
            else
            {
                _MaxHealth = _Health;
            }

            NotifyHealthStatusChanged();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                if (_Lives > 0)
                {
                    ConvertLifeToHealth();
                }
                else
                {
                    Die();
                }
            }

            NotifyHealthStatusChanged();
        }

        private void ConvertLifeToHealth()
        {
            Lives--;

            Health = _MaxHealth;
        }

        private void Die()
        {
            if (PlayAnimationOnDeath)
            {
                var a = transform.root.GetComponentInChildren<Animator>();

                if (a != null)
                {
                    a.SetTrigger("Die");
                }

                var c = GetComponent<ControllerOrchestrator>();

                if (c != null)
                {
                    c.enabled = false;
                }
            }
            else
            {
                Destroy(ObjectToDestroy);
            }
        }

        private void NotifyHealthStatusChanged()
        {
            _HealthStatusChangedEvent?.Invoke((_Lives, _Health));
        }
    }
}
