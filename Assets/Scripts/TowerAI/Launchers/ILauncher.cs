using System;
using UnityEngine;

namespace AirTowerDefence.Tower
{
    public interface ILauncher
    {
        event Action Launch;

        void Fire(Transform target);

        void Fire(Transform[] targets);
    }
}