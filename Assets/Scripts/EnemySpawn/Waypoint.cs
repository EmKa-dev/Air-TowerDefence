using UnityEngine;

namespace AirTowerDefence.EnemySpawn
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField]
        private Waypoint _Next;

        public Waypoint Next
        {
            get { return _Next; }
            set { _Next = value; }
        }
    }
}
