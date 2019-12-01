using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.Player
{
    public class TowerBuildingScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject _GhostPrefab;
        [SerializeField]
        private AimingScript _AimingScript;

        private bool IsPlacingTower;
        private GameObject _GhostThatIsBeingPlaced;

        void Update()
        {

            if (IsPlacingTower)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ExitBuildingMode();
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    BuildTower();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && !IsPlacingTower)
            {
                EnterBuildMode();
            }
        }

        private void BuildTower()
        {
            //GetComponent placementscript
            //Query placementscript for validity of placement
            var ghost = _GhostThatIsBeingPlaced.GetComponentInChildren<GhostPlacementScript>();

            if (ghost.IsPlacementValid)
            {
                Instantiate(ghost.ActualBuilding, _GhostThatIsBeingPlaced.transform.position, _GhostThatIsBeingPlaced.transform.rotation);

                ExitBuildingMode();
            }
        }

        private void EnterBuildMode()
        {
            IsPlacingTower = true;

            _GhostThatIsBeingPlaced = Instantiate(_GhostPrefab);

            _AimingScript.ChangeMarker(_GhostThatIsBeingPlaced);
        }

        private void ExitBuildingMode()
        {
            IsPlacingTower = false;
            _AimingScript.ChangeToOriginalMarker();
        }
    }
}
