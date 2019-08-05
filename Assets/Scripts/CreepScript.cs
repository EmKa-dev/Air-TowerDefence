using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreepScript : MonoBehaviour
{
    public int Health;

    public float Speed;

    [SerializeField]
    private SquareWaypoint TargetWayPoint;

    [SerializeField]
    private Vector3 TargetPositionInWorldSpace;

    [SerializeField]
    private Vector3 RelativePosition;

    void Start()
    {
        FindSpawnPoint();
        TargetPositionInWorldSpace = transform.position;
        RelativePosition = TargetWayPoint.transform.InverseTransformPoint(transform.position);
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
        }

        Vector3 targetvector = Vector3.MoveTowards(transform.position, TargetPositionInWorldSpace, Speed);

        transform.position = targetvector;
    }

    private void GetNextTargetPosition()
    {
        if (TargetWayPoint.Next != null)
        {
            TargetWayPoint = TargetWayPoint.Next;
        }

        TargetPositionInWorldSpace = TargetWayPoint.transform.TransformPoint(RelativePosition);
    }
}
