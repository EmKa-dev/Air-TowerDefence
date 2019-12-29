using AirTowerDefence.Common;
using UnityEngine;
using UnityEngine.UI;

namespace AirTowerDefence.UI
{
    public class HealthStatusTextUpdater : MonoBehaviour
    {
        [SerializeField]
        private Healthpool _TrackedHealthPool;

        [SerializeField]
        private Text _LivesText;

        [SerializeField]
        private Text _HealthText;

        void Awake()
        {
            if (_TrackedHealthPool != null)
            {
                _TrackedHealthPool._HealthStatusChangedEvent += OnStatusChange;

                _LivesText.text = _TrackedHealthPool.Lives.ToString();
                _HealthText.text = _TrackedHealthPool.Health.ToString();

            }


        }

        private void OnStatusChange((int lives, int health) tuple)
        {
            _LivesText.text = tuple.lives.ToString();
            _HealthText.text = tuple.health.ToString();

        }

        private void OnDestroy()
        {
            _TrackedHealthPool._HealthStatusChangedEvent -= OnStatusChange;
        }
    }
}
