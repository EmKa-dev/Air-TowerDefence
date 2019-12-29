using AirTowerDefence.Player;
using UnityEngine;
using UnityEngine.UI;

namespace AirTowerDefence.UI
{
    public class GoldBalanceTextUpdater : MonoBehaviour
    {

        [SerializeField]
        private MoneyBank _TrackedMoneyBank;

        [SerializeField]
        private Text _GoldBalanceText;

        void Awake()
        {
            _TrackedMoneyBank.BalanceChangedEvent += OnBalanceChange;

            _GoldBalanceText.text = _TrackedMoneyBank.Balance.ToString();
        }

        private void OnBalanceChange(int newbalance)
        {
            _GoldBalanceText.text = newbalance.ToString();
        }

        private void OnDestroy()
        {
            _TrackedMoneyBank.BalanceChangedEvent -= OnBalanceChange;
        }
    }
}
