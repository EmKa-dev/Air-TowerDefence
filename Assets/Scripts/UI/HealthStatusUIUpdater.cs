using AirTowerDefence.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace AirTowerDefence.UI
{
    public class HealthStatusUIUpdater : MonoBehaviour
    {
        [SerializeField]
        private Healthpool _TrackedHealthPool;

        [SerializeField]
        private Text _LivesText;

        [SerializeField]
        private GameObject[] _HealthBars;

        void Awake()
        {
            if (_TrackedHealthPool != null)
            {
                _TrackedHealthPool._HealthStatusChangedEvent += OnStatusChange;
            }
        }

        private void OnStatusChange((int lives, int health) tuple)
        {
            _LivesText.text = tuple.lives.ToString();
            //Health doesn't always reduce/add in sequence (ex. from 3 to 1 health and vice versa.)
            DeactivateAllHealthBars();
            ActivateHealthbars(tuple.health);
        }

        private void ActivateHealthbars(int health)
        {
            for (int i = 0; i < health; i++)
            {
                _HealthBars[i].SetActive(true);
            }
        }

        private void DeactivateAllHealthBars()
        {
            for (int i = 0; i < _HealthBars.Length; i++)
            {
                _HealthBars[i].SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _TrackedHealthPool._HealthStatusChangedEvent -= OnStatusChange;
        }
    }
}
