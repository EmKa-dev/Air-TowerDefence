using System;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    [RequireComponent(typeof(ControllerOrchestrator))]
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
