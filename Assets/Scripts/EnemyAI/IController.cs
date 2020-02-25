using System;

namespace AirTowerDefence.Enemy.Controllers
{
    public interface IController
    {
        bool IsDefaultController { get; }

        event Action<IController> RequestControl;

        event Action RelinquishControl;

        void UpdateControl();

        void OnControlLost();
    }
}
