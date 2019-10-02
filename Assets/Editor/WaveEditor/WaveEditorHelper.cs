using System.Collections.Generic;
using UnityEngine;

internal static class WaveEditorHelper
{
    internal static int CalculateRows(int columns, int elementsamount)
    {
        int buttonstodraw = elementsamount;

        int rows = 0;

        while (buttonstodraw > 0)
        {
            buttonstodraw -= columns;
            rows++;
        }

        if (rows == 0)
        {
            rows = 1;
        }

        return rows;
    }

    internal static GameObject BuildBareMeshCopy(GameObject original)
    {

        var go = GameObject.Instantiate(original);
        go.name = original.name;

        go.transform.localScale = original.transform.lossyScale;

        RemoveAllComponentsExceptMeshFilterAndMeshRenderInTransformTree(go.transform);

        return go;

        void RemoveAllComponentsExceptMeshFilterAndMeshRenderInTransformTree(Transform Root)
        {
            var components = go.GetComponentsInChildren<Component>();

            foreach (var component in components)
            {
                var componenttype = component.GetType();

                if (componenttype != typeof(MeshFilter) && componenttype != typeof(MeshRenderer) && componenttype != typeof(Transform))
                {
                    Object.DestroyImmediate(component);
                }
            }
        }
    }


}
