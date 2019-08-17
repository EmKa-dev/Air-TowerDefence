using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float ProjectileSpeed;

    [SerializeField]
    private ParticleSystem HitEffect;

    private Transform Target;


    public void Initialize(Transform target)
    {
        Target = target;
    }

    void Update()
    {
        if (Target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        Vector3 dir = Target.position - transform.position;

        float step = ProjectileSpeed * Time.deltaTime;

        if (dir.magnitude <= step)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * step, Space.World);
    }

    private void HitTarget()
    {
        Destroy(this.gameObject);
    }
}
