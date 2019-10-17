using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirTowerDefence.EditorTool
{
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

            var rootobject = new GameObject();

            rootobject.name = original.name;

            CopyTransformValues(original.transform ,rootobject.transform);

            foreach (var copygo in GetCopyOfTransformsWithMeshes(original))
            {
                copygo.transform.SetParent(rootobject.transform, true);
            }

            return rootobject;
        }


        //For every transform
        private static List<GameObject> GetCopyOfTransformsWithMeshes(GameObject original)
        {
            List<GameObject> meshcopies = new List<GameObject>();

            foreach (var transformwithmesh in original.GetComponentsInChildren<MeshFilter>())
            {
                meshcopies.Add(CreateCopyFromOrignal(transformwithmesh.gameObject));
            }

            return meshcopies;
        }

        private static GameObject CreateCopyFromOrignal(GameObject original)
        {
            GameObject copy = new GameObject();

            copy.name = $"{original.name}-copy";

            CopyTransformValues(original.transform, copy.transform);
            CopyMeshFromOrignal(original, copy);

            return copy;

        }

        private static void CopyTransformValues(Transform original, Transform copy)
        {
            copy.position = original.position;
            copy.rotation = original.rotation;
            copy.localScale = original.lossyScale;
        }

        private static void CopyMeshFromOrignal(GameObject original, GameObject destination)
        {

            var destinationmeshfilter = destination.AddComponent<MeshFilter>();
            var destinationrenderer = destination.AddComponent<MeshRenderer>();

            destinationmeshfilter.sharedMesh = original.GetComponent<MeshFilter>().sharedMesh;
            destinationrenderer.sharedMaterial = original.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}
