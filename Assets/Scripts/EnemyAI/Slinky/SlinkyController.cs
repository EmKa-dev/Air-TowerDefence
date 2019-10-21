using AirTowerDefence.EnemySpawn;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    public class SlinkyController : Controller
    {
        private Animator _Animator;

        [SerializeField]
        private Transform _ParentTransform;

        [SerializeField]
        private Transform _AnimationSteps;

        [SerializeField]
        private float TimeBetweenHops;

        [SerializeField]
        private float MaxDistanceToTargetBeforeNext;

        private SquareWaypoint TargetWayPoint;

        private Vector3 _TargetPositionInWorldSpace;
        private Vector3 _RelativePositionToWayPoint;

        private Vector3 _NewPosition;
        private Quaternion _NewRotation;

        private float HopTimer;

        void Start()
        {
            FindSpawnPoint();

            _NewPosition = _ParentTransform.position;
            HopTimer = TimeBetweenHops;

            _TargetPositionInWorldSpace = transform.position;
            _RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
        }

        private void FindSpawnPoint()
        {
            TargetWayPoint = GameObject.FindGameObjectWithTag("Spawnpoint").GetComponent<SquareWaypoint>();
        }

        private void Awake()
        {
            _Animator = GetComponentInChildren<Animator>();
        }

        public override void UpdateControl()
        {

            HopTimer -= Time.deltaTime;

            if (HopTimer <= 0f)
            {
                HopForward();
                HopTimer = TimeBetweenHops;
            }
        }

        private void HopForward()
        {
            //Play again from new position
            _ParentTransform.position = _NewPosition;
            _ParentTransform.rotation = _NewRotation;
            _Animator.Play("Slinky_HopForward", 0, 0f);
        }

        public void OnMiddleStepReached()
        {

        }

        public void OnAnimationEnd()
        {
            StoreNewPosition();

            if (IsCloseToTargetDestination())
            {
                GetNextTargetPosition();
                StoreNewRotation();
            }
        }

        private bool IsCloseToTargetDestination()
        {
            return Vector3.Distance(_NewPosition, _TargetPositionInWorldSpace) < MaxDistanceToTargetBeforeNext;
        }

        private void GetNextTargetPosition()
        {
            if (TargetWayPoint.Next != null)
            {
                TargetWayPoint = TargetWayPoint.Next;
            }

            _TargetPositionInWorldSpace = TargetWayPoint.transform.TransformPoint(_RelativePositionToWayPoint);
        }

        private void StoreNewPosition()
        {
            _NewPosition = _AnimationSteps.position;
        }
        private void StoreNewRotation()
        {
            _NewRotation = GetNewRotation();
        }

        private Quaternion GetNewRotation()
        {
            Quaternion lookrot = Quaternion.LookRotation(_TargetPositionInWorldSpace - _NewPosition);
            return Quaternion.Euler(0, lookrot.eulerAngles.y, 0);
        }
    }
}
