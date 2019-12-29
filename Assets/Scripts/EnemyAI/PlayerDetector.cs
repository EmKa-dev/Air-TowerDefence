using AirTowerDefence.Managers;
using System;
using UnityEngine;

namespace AirTowerDefence.Enemy
{
    public class PlayerDetector : MonoBehaviour
    {

        public event Action<Transform> TargetDetected;
        public event Action<Transform> TargetLost;

        [SerializeField]
        private float _Range;

        private bool TargetInSight = false;

        private static Transform _Player;


        private void Start()
        {
            if (_Player == null)
            {
                _Player = GameManager.Instance.Player;
            }
        }

        public bool IsPlayerWithinRange()
        {
            return Vector3.Distance(transform.position, _Player.position) < _Range;
        }

        private void Update()
        {
            if (!TargetInSight)
            {
                if (IsPlayerWithinRange())
                {
                    OnTargetAcquired();
                }
            }
            else if (TargetInSight)
            {
                if (!IsPlayerWithinRange())
                {
                    OnTargetLost();
                }
            }
        }

        private void OnTargetAcquired()
        {
            TargetInSight = true;
            TargetDetected?.Invoke(_Player);
        }

        private void OnTargetLost()
        {
            TargetInSight = false;
            TargetLost?.Invoke(_Player);
        }
    }
}
