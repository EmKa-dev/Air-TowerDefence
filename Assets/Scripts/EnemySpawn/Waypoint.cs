using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AirTowerDefence.EnemySpawn
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField]
        public Waypoint Next;
    }
}
