using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileLauncher : Launcher
{

    [SerializeField]
    private float _ProjectileRisingSpeed;

    [SerializeField]
    private float _ProjectileRisingTimer;

    public override void Fire(Transform target)
    {
        if (target == null)
        {
            return;
        }

        base.OnLaunch();

        var bullet = Instantiate(_ProjectilePrefab, _Muzzle.position, _Muzzle.rotation);
        bullet.GetComponent<HomingMissileProjectile>().enabled = false;
        StartCoroutine("ShootStraightUpBeforePursuit", (target, bullet));

    }

    private IEnumerator ShootStraightUpBeforePursuit((Transform target, GameObject bullet) tuple)
    {
        Vector3 targetPosition = tuple.target.position;

        float t = _ProjectileRisingTimer;

        float step = _ProjectileRisingSpeed * Time.deltaTime;

        while (t > 0f)
        {
            tuple.bullet.transform.Translate(Vector3.up.normalized * step, Space.World);

            t -= Time.deltaTime;

            if (tuple.target != null)
            {
                targetPosition = tuple.target.position;
            }

            yield return null;
        }

        if (tuple.target == null)
        {
            tuple.bullet.GetComponent<HomingMissileProjectile>().Initialize(targetPosition, _Damage, _ProjectileSpeed);
        }
        else
        {
            tuple.bullet.GetComponent<HomingMissileProjectile>().Initialize(tuple.target, _Damage, _ProjectileSpeed);
        }

        tuple.bullet.GetComponent<HomingMissileProjectile>().enabled = true;
    }
}
