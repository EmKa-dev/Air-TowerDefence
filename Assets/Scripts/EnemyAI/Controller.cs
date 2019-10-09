using System;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    public abstract class Controller : MonoBehaviour, IController
    {
        public event Action<IController> RequestControl;

        public abstract void UpdateControl();

        protected void InvokeRequestControl()
        {
            RequestControl?.Invoke(this);
        }
    }
}
