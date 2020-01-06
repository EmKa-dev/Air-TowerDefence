using UnityEngine;

namespace AirTowerDefence.Managers
{
    public class GhostPlacementScript : MonoBehaviour
    {
        public bool IsPlacementValid { get; private set; }

        void Update()
        {
            IsPlacementValid = CheckIfPlacementIsValid();

        }

        private bool CheckIfPlacementIsValid()
        {
            //TODO:
            //Check placement conditions for example is on terrain, doesnt collide with other objects etc. 
            return true;
        }
    }
}
