using AirTowerDefence.Enemy.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerOrchestrator : MonoBehaviour
{
    private IController _ActiveController;

    void Start()
    {
        var controllers = transform.root.GetComponentsInChildren<IController>();

        foreach (var controller in controllers)
        {
            controller.RequestControl += OnControllerRequest;
        }
    }

    private void OnControllerRequest(IController controller)
    {
        _ActiveController = controller;
    }

    void Update()
    {
        _ActiveController?.UpdateControl();
    }
}
