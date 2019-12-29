using UnityEngine;

namespace AirTowerDefence.Projectile
{
    internal interface IProjectile
    {
        void Initialize(Transform target, int damage, float speed);
    }
}

