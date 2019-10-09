using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.EnemySpawn
{
    [Serializable]
    public class Spawn
    {
        public List<SpawnData> SpawnData = new List<SpawnData>();

        [SerializeField]
        private int _TimeToNext;

        public int TimeToNext
        {
            get { return _TimeToNext; }
            set
            {
                if (value < 0)
                {
                    _TimeToNext = 0;
                }
                else if (value > 99)
                {
                    _TimeToNext = 99;
                }
                else
                {
                    _TimeToNext = value;
                }
            }
        }
    }
}
