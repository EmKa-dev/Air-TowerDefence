using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreepScript : MonoBehaviour, IDamagable
{
    [SerializeField]
    private float Health;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private SquareWaypoint TargetWayPoint;

    [SerializeField]
    private Vector3 TargetPositionInWorldSpace;

    [SerializeField]
    private Vector3 RelativePositionToWayPoint;

    void Start()
    {
        FindSpawnPoint();
        TargetPositionInWorldSpace = transform.position;
        RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        MoveTowardTargetWayPoint();
    }

    private void FindSpawnPoint()
    {
        TargetWayPoint = GameObject.FindGameObjectWithTag("Spawnpoint").GetComponent<SquareWaypoint>();
    }

    private void MoveTowardTargetWayPoint()
    {
        if (Vector3.Distance(transform.position, TargetPositionInWorldSpace) < 0.001f)
        {
            GetNextTargetPosition();
            RotateTowardsTarget();
        }

        Vector3 targetvector = Vector3.MoveTowards(transform.position, TargetPositionInWorldSpace, Speed * Time.deltaTime);

        transform.position = targetvector;
    }

    private void RotateTowardsTarget()
    {
        Vector3 dir = TargetWayPoint.transform.position - transform.position;

        Quaternion lookrot = Quaternion.LookRotation(dir);
        transform.rotation = lookrot;
    }

    private void GetNextTargetPosition()
    {
        if (TargetWayPoint.Next != null)
        {
            TargetWayPoint = TargetWayPoint.Next;
        }

        TargetPositionInWorldSpace = TargetWayPoint.transform.TransformPoint(RelativePositionToWayPoint);
    }

    public void TakeDamage(float damage)
    {
        this.Health -= damage;

        if (Health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
