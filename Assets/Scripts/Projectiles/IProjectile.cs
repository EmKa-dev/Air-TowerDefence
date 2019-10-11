using UnityEngine;

namespace AirTowerDefence.Projectile
{
    internal interface IProjectile
    {
        void Initialize(Transform target, float damage, float speed);
    }
}

