using System;
using System.Collections.Generic;

namespace AirTowerDefence.EnemySpawn
{
    [Serializable]
    public class Wave
    {
        public List<Spawn> Spawns = new List<Spawn>();
    }
}
