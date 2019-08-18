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
    private float FireTimer = 1;

    void Awake()
    {
        CreepLayerMask = LayerMask.GetMask("Creep");
        CandiateResults = new Collider[10];
        Targets = new Transform[TargetsAmount];
        InvokeRepeating("UpdateTargets", 2f, SearchRate);
    }

    void Update()
    {
        FireTimer -= Time.deltaTime;
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
        // Note: If overlapsphere finds nothing, nothing in the buffer gets overwritten,
        // which means a previous candidate might have moved out of range, but still remain in the buffer.
        // therefore we must do rangechecks even though one would assume the buffer only contains results within range..

        Physics.OverlapSphereNonAlloc(transform.position, Range, CandiateResults, CreepLayerMask);

        for (int i = 0; i < Targets.Length; i++)
        {
            //skip if already held target is not null and within range
            if (Targets[i] != null && Vector3.Distance(transform.position, Targets[i].transform.position) <= Range)
            {
                continue;
            }

            Targets[i] = FindValidTargetFromCandidatesOrNullIfNone();
        }

        Transform FindValidTargetFromCandidatesOrNullIfNone()
        {
            for (int i = 0; i < CandiateResults.Length; i++)
            {

                if (CandiateResults[i] != null && IsThisWithinRange() && !IsThisTakenByAnotherSlot())
                {
                    return CandiateResults[i].transform;
                }


                bool IsThisWithinRange()
                {
                    return Vector3.Distance(transform.position, CandiateResults[i].transform.position) <= Range;
                }

                bool IsThisTakenByAnotherSlot()
                {
                    return Targets.Any(x => Object.ReferenceEquals(x, CandiateResults[i].transform));
                }
            }

            return null;
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
