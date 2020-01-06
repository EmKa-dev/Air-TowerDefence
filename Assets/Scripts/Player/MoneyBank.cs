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


		private void Start()
		{
			Balance = 2000;
		}
		private void OnBalanceChanged()
		{
			BalanceChangedEvent?.Invoke(_Balance);
		}

		public void SubtractFromBalance(int amount)
		{
			Balance -= amount;
		}

		public void AddToBalance(int amount)
		{
			Balance += amount;
		}
	}
}
