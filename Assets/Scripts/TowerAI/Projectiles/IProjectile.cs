using UnityEngine;

internal interface IProjectile
{
    void Initialize(Transform target, float damage, float speed);
}

