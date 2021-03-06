﻿using AirTowerDefence.Projectile;
using System;
using UnityEngine;

namespace AirTowerDefence.Tower
{
    public class Launcher : MonoBehaviour, ILauncher
    {
        [Header("Attributes")]

        [SerializeField]
        protected int _Damage;

        [SerializeField]
        protected float _ProjectileSpeed;

        [Header("Setup")]

        [SerializeField]
        protected Transform _Muzzle;

        [SerializeField]
        protected GameObject _ProjectilePrefab;

        public event Action Launch;

        public virtual void Fire(Transform[] targets)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                {
                    FireProjectile(targets[i]);
                }
            }
        }

        public virtual void Fire(Transform target)
        {
            if (target != null)
            {
                FireProjectile(target);
            }
        }

        private void FireProjectile(Transform target)
        {
            var bullet = Instantiate(_ProjectilePrefab, _Muzzle.position, _Muzzle.rotation);
            bullet.GetComponent<IProjectile>().Initialize(target, _Damage, _ProjectileSpeed);

            OnLaunch();
        }

        protected void OnLaunch()
        {
            Launch?.Invoke();
        }
    }
}

