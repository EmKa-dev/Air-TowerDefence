using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem HitEffect;

    [SerializeField]
    private float _Speed;

    void Update()
    {
        transform.position += transform.forward * _Speed * Time.deltaTime;
    }
}
