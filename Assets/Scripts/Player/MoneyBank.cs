using System;
using UnityEngine;

namespace AirTowerDefence.Player
{
    public class MoneyBank : MonoBehaviour
    {

		public event Action<int> BalanceChangedEvent;

		private int _Balance;
		public int Balance
		{
			get { return _Balance; }
			private set
			{
				_Balance = value;
				OnBalanceChanged();
			}
		}

		private void OnBalanceChanged()
		{
			BalanceChangedEvent?.Invoke(_Balance);
		}
	}
}
