using System;
using UnityEngine;

public class HomingMissileProjectile : MonoBehaviour, IProjectile
{

    [SerializeField]
    private ParticleSystem _HitEffect;

    private Transform _Target;

    private float _Damage;

    private float _Speed;

    [SerializeField]
    private Vector3 _LastKnownTargetPosition;

    public void Initialize(Transform target, float damage, float speed)
    {
        _Target = target;
        _Damage = damage;
        _Speed = speed;
        _LastKnownTargetPosition = target.position;
    }

    public void Initialize(Vector3 lastknownposition, float damage, float speed)
    {
        _Damage = damage;
        _Speed = speed;
        _LastKnownTargetPosition = lastknownposition;
    }

    void Update()
    {

        if (_Target != null)
        {
            _LastKnownTargetPosition = _Target.position;
        }

        Vector3 dir = _LastKnownTargetPosition - transform.position;
        float step = _Speed * Time.deltaTime;

        if (dir.magnitude > step)
        {
            dir = _LastKnownTargetPosition - transform.position;
            //Rotate bullet
            Quaternion lookrot = Quaternion.LookRotation(dir);
            transform.rotation = lookrot;
            transform.Rotate(90, 0, 0);

            transform.Translate(dir.normalized * step, Space.World);

            return;
        }

        HitTarget();
    }

    private void Explode()
    {
        if (_HitEffect != null)
        {
            var effect = Instantiate(_HitEffect, _LastKnownTargetPosition + (Vector3.up * 0.5f), Quaternion.identity);
            effect.Play();
        }

        Destroy(gameObject);
    }

    private void HitTarget()
    {
        if (_Target != null)
        {
            _Target.GetComponent<IDamagable>().TakeDamage(_Damage);
        }

        Explode();
    }
}
