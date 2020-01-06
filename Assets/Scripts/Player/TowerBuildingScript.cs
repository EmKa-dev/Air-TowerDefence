using AirTowerDefence.Managers;
using UnityEngine;

namespace AirTowerDefence.Player
{
    public class TowerBuildingScript : MonoBehaviour
    {       
        [SerializeField]
        private LayerMask _LayerMask;

        [SerializeField]
        private RectTransform _CrossHairCanvas;

        private MoneyBank _PlayerBank;
        private GameObject _Camera;

        private bool IsInBuildMode = false;

        private GameObject _GhostThatIsBeingPlaced;

        private void Awake()
        {
            _Camera = Camera.main.gameObject;
            _PlayerBank = GetComponent<MoneyBank>();
        }

        void Update()
        {
            if (IsInBuildMode)
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
            else
            {
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (Shop.Instance.TryGetSelectedGhost(_PlayerBank, out GameObject res))
                    {
                        EnterBuildMode(res);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    Shop.Instance.SwitchSelectedItem();
                }
            }
        }

        private void EnterBuildMode(GameObject ghost)
        {
            IsInBuildMode = true;

            _GhostThatIsBeingPlaced = Instantiate(ghost);

            _CrossHairCanvas.gameObject.SetActive(false);
        }

        //TODO: Require call to shop (CommitPurchase) to get actual building.
        private void BuildTower()
        {
            //Propsed: Don't keep the actual/real building in the GhostScript, which should exclusively be for checking valid placement
            var ghost = _GhostThatIsBeingPlaced.GetComponentInChildren<GhostPlacementScript>();

            if (ghost.IsPlacementValid)
            {
                var building = Shop.Instance.CommitPurchase(_PlayerBank);

                Instantiate(building, _GhostThatIsBeingPlaced.transform.position, _GhostThatIsBeingPlaced.transform.rotation);

                ExitBuildMode();
            }
        }

        private void ExitBuildMode()
        {
            IsInBuildMode = false;

            Destroy(_GhostThatIsBeingPlaced);
            _CrossHairCanvas.gameObject.SetActive(true);
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
