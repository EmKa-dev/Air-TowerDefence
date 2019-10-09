using System.Linq;
using UnityEngine;

namespace AirTowerDefence.Tower
{
    public class MultiTargetter : Targetter, IMultiTargetBehaviour
    {

        [SerializeField]
        private int TargetsAmount;

        public Transform[] Targets { get; set; }

        private float UpdateTimer;

        void Awake()
        {
            _TargetLayerMask = UnityEngine.LayerMask.GetMask("Creep");
            _CandiateBuffer = new Collider[10];
            Targets = new Transform[TargetsAmount];
        }

        void Update()
        {

            UpdateTimer -= Time.deltaTime;

            if (UpdateTimer <= 0)
            {
                Search();
                UpdateTimer = this.SearchRate;
            }

            if (Targets.All(x => x == null))
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
            var target = Targets.FirstOrDefault(x => x != null);

            if (target != null)
            {
                Vector3 dir = target.position - _TurretMesh.transform.position;

                Quaternion lookrot = Quaternion.LookRotation(dir);
                _TurretMesh.transform.rotation = lookrot;
            }
        }

        public override void Search()
        {
            // Note: If overlapsphere finds nothing, nothing in the buffer gets overwritten,
            // which means a previous candidate might have moved out of range, but still remain in the buffer.
            // therefore we must do rangechecks even though one would assume the buffer only contains results within range..

            Physics.OverlapSphereNonAlloc(transform.position, Range, _CandiateBuffer, _TargetLayerMask);

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
                for (int i = 0; i < _CandiateBuffer.Length; i++)
                {

                    if (_CandiateBuffer[i] != null && IsThisWithinRange() && !IsThisTakenByAnotherSlot())
                    {
                        return _CandiateBuffer[i].transform;
                    }


                    bool IsThisWithinRange()
                    {
                        return Vector3.Distance(transform.position, _CandiateBuffer[i].transform.position) <= Range;
                    }

                    bool IsThisTakenByAnotherSlot()
                    {
                        return Targets.Any(x => Object.ReferenceEquals(x, _CandiateBuffer[i].transform));
                    }
                }

                return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (_TurretMesh == null || Targets == null)
            {
                return;
            }

            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i] != null)
                {
                    Gizmos.DrawLine(_TurretMesh.transform.position, Targets[i].position);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Range);
        }
    }
}
