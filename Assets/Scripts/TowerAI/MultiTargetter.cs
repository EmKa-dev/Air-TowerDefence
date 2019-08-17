using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiTargetter : MonoBehaviour
{
    [Header("Attributes")]

    public float Range;

    public float SearchRate;

    public float FireRate;

    public int TargetsAmount;

    [Header("Setup")]

    [SerializeField]
    private GameObject TurretMesh;

    [SerializeField]
    private Transform Muzzle;

    [SerializeField]
    private GameObject ProjectilePrefab;

    [Header("VisualTesting")]

    public Transform[] Targets;

    [SerializeField]
    private Collider[] CandiateResults;

    private int CreepLayerMask;
    private float FireTimer = 0;

    void Awake()
    {
        CreepLayerMask = LayerMask.GetMask("Creep");
        CandiateResults = new Collider[10];
        Targets = new Transform[TargetsAmount];
        InvokeRepeating("UpdateTargets", 2f, SearchRate);
    }

    void Update()
    {
        if (Targets.All(x => x == null))
        {
            return;
        }

        RotateTowardsTarget();

        if (FireTimer <= 0)
        {
            FireProjectiles();
            FireTimer = FireRate;
        }

        FireTimer -= Time.deltaTime;
    }

    private void RotateTowardsTarget()
    {
        var target = Targets.FirstOrDefault(x => x != null);

        if (target != null)
        {
            Vector3 dir = target.position - TurretMesh.transform.position;

            Quaternion lookrot = Quaternion.LookRotation(dir);
            TurretMesh.transform.rotation = lookrot;
        }
    }

    public void UpdateTargets()
    {
        Physics.OverlapSphereNonAlloc(transform.position, Range, CandiateResults, CreepLayerMask);

        //var orderedlistofvalidtargets = CandiateResults.Where(z => z != null && Vector3.Distance(transform.position, z.transform.position) <= Range)
        //    .OrderByDescending(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();

        for (int i = 0; i < Targets.Length; i++)
        {
            //skip if already held target is not null and within range
            if (Targets[i] != null && Vector3.Distance(transform.position, Targets[i].transform.position) <= Range)
            {
                continue;
            }

            Targets[i] = FindNearestAndRemoveFromCandidates();
        }

        Transform FindNearestAndRemoveFromCandidates()
        {
            float shortestdistance = Mathf.Infinity;

            int nearestenemyindex = -1;
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
                        nearestenemyindex = i;
                    }
                }
            }

            if (nearestenemy != null && shortestdistance <= Range && !IsThisTakenByAnotherSlot())
            {
                CandiateResults[nearestenemyindex] = null;
                return  nearestenemy.transform;
            }
            else
            {
                return  null;
            }

            bool IsThisTakenByAnotherSlot()
            {
                if (Targets.Any(x => Object.ReferenceEquals(x, nearestenemy.transform)))
                {
                    return true;
                }

                return false;
            }
        }


    }

    private void FireProjectiles()
    {
        for (int i = 0; i < Targets.Length; i++)
        {
            if (Targets[i] != null)
            {
                var bullet = Instantiate(ProjectilePrefab, Muzzle.position, Muzzle.rotation);
                bullet.GetComponent<Projectile>().Initialize(Targets[i]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Targets == null)
        {
            return;
        }

        for (int i = 0; i < Targets.Length; i++)
        {
            if (Targets[i] != null)
            {
                Gizmos.DrawLine(TurretMesh.transform.position, Targets[i].position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
