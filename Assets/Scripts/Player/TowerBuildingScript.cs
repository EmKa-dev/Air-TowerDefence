using AirTowerDefence.Managers;
using UnityEngine;

//TODO
// Find a way to communicate which tower is being selected from the shop-panel

namespace AirTowerDefence.Player
{
    public class TowerBuildingScript : MonoBehaviour
    {
        
        [SerializeField]
        private LayerMask _LayerMask;

        [SerializeField]
        private RectTransform _CrossHair;

        private GameObject _Camera;

        private bool IsPlacingTower;

        private GameObject _GhostThatIsBeingPlaced;


        private void Awake()
        {
            _Camera = Camera.main.gameObject;
        }

        void Update()
        {

            if (IsPlacingTower)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ExitBuildMode();
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    BuildTower();
                }

                PositionGhostAtAim();

            }

            if (Input.GetKeyDown(KeyCode.B) && !IsPlacingTower)
            {
                EnterBuildMode(Shop.Instance.SelectedItem.GhostPrefab);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Shop.Instance.SwitchSelectedItem();
            }
        }

        private void BuildTower()
        {
            //Query placementscript for validity of placement
            var ghost = _GhostThatIsBeingPlaced.GetComponentInChildren<GhostPlacementScript>();

            if (ghost.IsPlacementValid)
            {
                Instantiate(ghost.ActualBuilding, _GhostThatIsBeingPlaced.transform.position, _GhostThatIsBeingPlaced.transform.rotation);

                ExitBuildMode();
            }
        }

        private void EnterBuildMode(GameObject ghost)
        {
            IsPlacingTower = true;

            _GhostThatIsBeingPlaced = Instantiate(ghost);

            _CrossHair.gameObject.SetActive(false);
        }

        private void ExitBuildMode()
        {
            IsPlacingTower = false;

            Destroy(_GhostThatIsBeingPlaced);
            _CrossHair.gameObject.SetActive(true);
        }

        private void PositionGhostAtAim()
        {
            Ray ray = new Ray(_Camera.transform.position, _Camera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _LayerMask))
            {
                _GhostThatIsBeingPlaced.transform.position = hit.point;
                _GhostThatIsBeingPlaced.transform.position = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            }
        }
    }
}
