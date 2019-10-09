using System;

namespace AirTowerDefence.Enemy.Controllers
{
    public interface IController
    {
        event Action<IController> RequestControl;

        void UpdateControl();
    }
}
