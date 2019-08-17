using System;
using UnityEngine;

public class SingleTargetter : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    private float Range;

    [SerializeField]
    private float SearchRate;

    [SerializeField]
    private float FireRate;

    [Header("Setup")]

    [SerializeField]
    private GameObject TurretMesh;

    [SerializeField]
    private Transform Muzzle;

    [SerializeField]
    private GameObject ProjectilePrefab;

    [Header("VisualTesting")]

    [SerializeField]
    private Transform Target;

    [SerializeField]
    private Collider[] CandiateResults;

    private int CreepLayerMask;
    private float FireTimer = 0;

    void Start()
    {
        CreepLayerMask = LayerMask.GetMask("Creep");
        CandiateResults = new Collider[10];
        InvokeRepeating("UpdateTarget", 2f, SearchRate);
    }

    void Update()
    {
        if (Target == null)
        {
            return;
        }

        RotateTowardsTarget();

        if (FireTimer <= 0)
        {
            FireProjectile();
            FireTimer = FireRate;
        }

        FireTimer -= Time.deltaTime;
    }

    private void RotateTowardsTarget()
    {
        Vector3 dir = Target.position - TurretMesh.transform.position;

        Quaternion lookrot = Quaternion.LookRotation(dir);
        TurretMesh.transform.rotation = lookrot;
    }

    public void UpdateTarget()
    {
        //If current target is still alive and within range, do nothing
        if (Target != null && Vector3.Distance(transform.position, Target.position) < Range)
        {
            return;
        }

        Physics.OverlapSphereNonAlloc(transform.position, Range, CandiateResults, CreepLayerMask);

        float shortestdistance = Mathf.Infinity;
        Collider nearestenemy = null;

        for (int i = 0; i < CandiateResults.Length; i++)
        {
            if (CandiateResults[i] != null)
            {
                var distancetoenemy = Vector3.Distance(transform.position, CandiateResults[i].transform.position);

                if (distancetoenemy < shortestdistance)
                {
                    shortestdistance = distancetoenemy;
                    nearestenemy = CandiateResults[i];
                }
            } 
        }

        if (nearestenemy != null && shortestdistance <= Range)
        {
            Target = nearestenemy.transform;
        }
        else
        {
            Target = null;
        }
        
    }

    private void FireProjectile()
    {
        var bullet = Instantiate(ProjectilePrefab, Muzzle.position, Muzzle.rotation);
        bullet.GetComponent<Projectile>().Initialize(Target);

    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.DrawLine(TurretMesh.transform.position, Target.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}

