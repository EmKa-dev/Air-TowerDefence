using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ImmutableGameObject : MonoBehaviour
{
    private Vector3 OriginalPosition;

    private void OnEnable()
    {
        OriginalPosition = transform.position;
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
    }

    private void DisableToolHandlersIfSelected()
    {
        Tools.hidden = Selection.activeGameObject == this.gameObject ? true : false;

    }
}
