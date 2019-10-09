using UnityEngine;

namespace AirTowerDefence.Tower
{
    internal interface IMultiTargetBehaviour
    {
        Transform[] Targets { get; }
    }
}