﻿using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ImmutableGameObject : MonoBehaviour
{
    private Vector3 OriginalPosition;
    private Quaternion OriginalRotation;
    private Vector3 OriginalScale;

    private void OnEnable()
    {
        OriginalPosition = transform.position;
        OriginalRotation = transform.rotation;
        OriginalScale = transform.localScale;

    }

    private void OnRenderObject()
    {
        PreventMovement();
        DisableToolHandlersIfSelected();
    }

    private void PreventMovement()
    {
        if (transform.position != OriginalPosition)
        {
            transform.position = OriginalPosition;
        }

        if (transform.rotation != OriginalRotation)
        {
            transform.rotation = OriginalRotation;
        }

        if (transform.localScale != OriginalScale)
        {
            transform.localScale = OriginalScale;
        }
    }

    private void DisableToolHandlersIfSelected()
    {
        Tools.hidden = Selection.activeGameObject == this.gameObject ? true : false;

    }

    private void OnDestroy()
    {
        Tools.hidden = false;
    }
}