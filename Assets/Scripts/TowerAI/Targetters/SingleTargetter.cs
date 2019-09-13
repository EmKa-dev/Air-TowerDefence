using System;
using UnityEngine;

public class SingleTargetter : Targetter, ISingleTargetBehaviour
{

    [SerializeField]
    public Transform Target { get; set; }

    private float UpdateTimer;

    void Awake()
    {
        this.UpdateTimer = this.SearchRate;
        this._TargetLayerMask = UnityEngine.LayerMask.GetMask("Creep");
        this._CandiateBuffer = new Collider[5];
    }

    void Update()
    {
        UpdateTimer -= Time.deltaTime;

        if (UpdateTimer <= 0)
        {
            Search();
            UpdateTimer = this.SearchRate;
        }

        if (Target == null)
        {
            return;
        }

        if (_TurretMesh != null)
        {
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 dir = Target.position - _TurretMesh.transform.position;

        Quaternion lookrot = Quaternion.LookRotation(dir);
        _TurretMesh.transform.rotation = lookrot;
    }

    public override void Search()
    {
        //If current target is still alive and within range, do nothing
        if (Target != null && Vector3.Distance(transform.position, Target.position) < Range)
        {
            return;
        }

        Physics.OverlapSphereNonAlloc(transform.position, Range, _CandiateBuffer, _TargetLayerMask);

        float shortestdistance = Mathf.Infinity;
        Collider nearestenemy = null;

        for (int i = 0; i < _CandiateBuffer.Length; i++)
        {
            if (_CandiateBuffer[i] != null)
            {
                var distancetoenemy = Vector3.Distance(transform.position, _CandiateBuffer[i].transform.position);

                if (distancetoenemy < shortestdistance)
                {
                    shortestdistance = distancetoenemy;
                    nearestenemy = _CandiateBuffer[i];
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

    private void OnDrawGizmos()
    {
        if (_TurretMesh == null || Target == null)
        {
            return;
        }

        Gizmos.DrawLine(_TurretMesh.transform.position, Target.position);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}

