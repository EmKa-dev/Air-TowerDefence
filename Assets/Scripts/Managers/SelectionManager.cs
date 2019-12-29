using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.Managers
{
    public class SelectionManager : MonoBehaviour
    {

        [SerializeField]
        LayerMask _LayerMask;
        [SerializeField]
        private Material _SelectionMaterial;

        private GameObject _Camera;

        private Transform _CurrentSelection;

        private List<Renderer> _SelectedObjectRenderers = new List<Renderer>(5);

        private Material _DefaultMaterialForSelection;


        private void Awake()
        {
            _Camera = Camera.main.gameObject;
        }

        void Update()
        {

            RayCastFromCamera();
        }

        private void RayCastFromCamera()
        {
            Ray ray = new Ray(_Camera.transform.position, _Camera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _LayerMask))
            {

                if (object.ReferenceEquals(hit, _CurrentSelection))
                {
                    //do nothing
                    return;
                }
                else
                {

                    ClearSelection();
                }

                GetRenderers(hit.transform);
                SetSelectionMaterialOnRenderers();

            }
            else
            {
                ClearSelection();
            }

        }

        private void GetRenderers(Transform target)
        {
            _SelectedObjectRenderers.AddRange(target.transform.GetComponentsInChildren<Renderer>());
        }

        private void SetSelectionMaterialOnRenderers()
        {
            foreach (var renderer in _SelectedObjectRenderers)
            {
                _DefaultMaterialForSelection = renderer.material;
                renderer.material = _SelectionMaterial;
            }
        }

        private void SetDefaultMaterialOnRenderers()
        {
            foreach (var renderer in _SelectedObjectRenderers)
            {
                renderer.material = _DefaultMaterialForSelection;
            }
        }

        private void ClearSelection()
        {
            SetDefaultMaterialOnRenderers();

            _SelectedObjectRenderers.Clear();

            _CurrentSelection = null;
            _DefaultMaterialForSelection = null;
        }
    }
}
