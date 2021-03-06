﻿using AirTowerDefence.EnemySpawn;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    public class SlinkyController : MonoBehaviour, ISpawnableCreep
    {
        private Animator _Animator;

        private Transform _RootObjectTransform;

        [SerializeField]
        private Transform _AnimationSteps;

        [SerializeField]
        private float _TimeBetweenHops;

        [SerializeField]
        private float _HopSpeed;

        [SerializeField]
        private float MaxDistanceToTargetBeforeNext;

        private Waypoint TargetWayPoint;

        private Vector3 _TargetPositionInWorldSpace;
        private Vector3 _RelativePositionToWayPoint;

        private Vector3 _NewPosition;
        private Quaternion _NewRotation;

        private float _HopTimer;
        private bool _IsHopping;

        private void Awake()
        {
            _Animator = GetComponentInChildren<Animator>();
            _Animator.SetFloat("HopSpeed", _HopSpeed);

            _RootObjectTransform = transform.root;

            _HopTimer = Mathf.Infinity;
        }

        public void Initialize(Waypoint spawnpoint)
        {
            TargetWayPoint = spawnpoint;
            _TargetPositionInWorldSpace = transform.position;
            _RelativePositionToWayPoint = TargetWayPoint.transform.InverseTransformPoint(transform.position);
            _NewPosition = _RootObjectTransform.position;

            GetNextTargetPosition();
            StoreNewRotation();

            _HopTimer = _TimeBetweenHops;
        }

        void Update()
        {
            if (!_IsHopping)
            {
                _HopTimer -= Time.deltaTime;
            }

            if (_HopTimer <= 0f)
            {
                _IsHopping = true;
                HopForward();
                _HopTimer = _TimeBetweenHops;
            }
        }

        private void HopForward()
        {
            _RootObjectTransform.position = _NewPosition;
            _RootObjectTransform.rotation = _NewRotation;
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

            _IsHopping = false;
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
            var dir = _TargetPositionInWorldSpace - _NewPosition;

            if (dir == Vector3.zero)
            {
                return Quaternion.Euler(dir);

            }

            Quaternion lookrot = Quaternion.LookRotation(dir);
            return Quaternion.Euler(0, lookrot.eulerAngles.y, 0);
        }
    }
}
