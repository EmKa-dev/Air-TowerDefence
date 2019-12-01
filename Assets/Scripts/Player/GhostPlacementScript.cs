using AirTowerDefence.Player;
using UnityEngine;

namespace AirTowerDefence.Player
{
    public class GhostPlacementScript : MonoBehaviour
    {
        public bool IsPlacementValid { get; private set; }

        public GameObject ActualBuilding;

        void Start()
        {

        }

        void Update()
        {
            IsPlacementValid = true;

        }
    }
}
