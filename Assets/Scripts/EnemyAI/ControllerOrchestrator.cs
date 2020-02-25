using UnityEngine;
using System;

namespace AirTowerDefence.Enemy.Controllers
{

    public class ControllerOrchestrator : MonoBehaviour
    {
        private IController[] _Controllers;

        private IController _ActiveController;

        private int _DefaultControllerIndex;

        void Start()
        {
            _Controllers = transform.root.GetComponentsInChildren<IController>();

            foreach (var controller in _Controllers)
            {
                controller.RequestControl += OnControllerRequest;
                controller.RelinquishControl += OnControllerRelinqusish;
            }

            _DefaultControllerIndex = GetIndexOfDefaultController();
            _ActiveController = _Controllers[_DefaultControllerIndex];
        }

        private int GetIndexOfDefaultController()
        {
            for (int i = 0; i < _Controllers.Length; i++)
            {
                if (_Controllers[i].IsDefaultController)
                {
                    return i;
                }
            }

            throw new Exception($"Faulty setup in gameobject:{gameObject.name}, must have a default controller");
        }

        void Update()
        {
            _ActiveController?.UpdateControl();
        }

        private void OnControllerRequest(IController controller)
        {
            _ActiveController.OnControlLost();
            _ActiveController = controller;
        }

        private void OnControllerRelinqusish()
        {
            _ActiveController.OnControlLost();
            _ActiveController = _Controllers[_DefaultControllerIndex];
        }

        private void OnDestroy()
        {
            foreach (var controller in _Controllers)
            {
                controller.RequestControl -= OnControllerRequest;
                controller.RelinquishControl -= OnControllerRelinqusish;
            }
        }
    }
}
