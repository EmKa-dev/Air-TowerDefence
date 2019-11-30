using UnityEngine;

namespace AirTowerDefence.Tower
{
    public class AttackOrchestrator : MonoBehaviour
    {
        [Header("Setup")]

        [SerializeField]
        public float FireRate;

        private Targetter _TargetBehaviour;

        protected ILauncher _Launcher;

        private float FireTimer;

        private void Awake()
        {
            _TargetBehaviour = GetComponent<Targetter>();
            _Launcher = GetComponent<ILauncher>();

            if (_TargetBehaviour == null)
            {
                Debug.LogError($"Missing component {nameof(Targetter)}");
            }
            if (_Launcher == null)
            {
                Debug.LogError($"Missing component {nameof(ILauncher)}");
            }

            ResetFireTimer();
        }

        private void Update()
        {
            FireTimer -= Time.deltaTime;

            if (FireTimer <= 0f)
            {
                if (_TargetBehaviour is ISingleTargetBehaviour s)
                {
                    _Launcher.Fire(s.Target);
                }
                else if (_TargetBehaviour is IMultiTargetBehaviour m)
                {
                    _Launcher.Fire(m.Targets);
                }

                ResetFireTimer();
            }
        }

        private void ResetFireTimer()
        {
            FireTimer = 1 / FireRate;
        }
    }
}

