using System;
using UnityEngine;

namespace AirTowerDefence.EnemySpawn
{
    [Serializable]
    public struct SpawnData
    {
        public string CreepIdentifier;
        public Vector3 RelativePosition;
    }
}
