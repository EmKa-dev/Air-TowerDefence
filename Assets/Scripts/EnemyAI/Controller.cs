using System;
using UnityEngine;

namespace AirTowerDefence.Enemy.Controllers
{
    [RequireComponent(typeof(ControllerOrchestrator))]
    public abstract class Controller : MonoBehaviour, IController
    {

        [SerializeField]
        private bool _IsDefaultController;

        public bool IsDefaultController
        {
            get { return _IsDefaultController; }
        }

        public event Action<IController> RequestControl;

        public event Action RelinquishControl;

        public abstract void UpdateControl();

        protected void InvokeRequestControl()
        {
            RequestControl?.Invoke(this);
        }

        protected void InvokeRelinquishControl()
        {
            RelinquishControl?.Invoke();
        }

        public virtual void OnControlLost()
        {
        }
    }
}
