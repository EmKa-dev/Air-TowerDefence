using System.Collections.Generic;
using System.Linq;
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

            var listofcopies = GetCopyOfTransformsWithMeshes(original);

            if (listofcopies.Count == 1)
            {
                return listofcopies.First();
            }

            var copycontainer = new GameObject();

            copycontainer.name = original.name;

            CopyTransformValues(original.transform , copycontainer.transform);

            foreach (var copygo in listofcopies)
            {
                copygo.transform.SetParent(copycontainer.transform, true);
            }

            return copycontainer;
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

        private static void CopyTransformValues(Transform original, Transform destination)
        {
            destination.position = original.position;
            destination.rotation = original.rotation;
            destination.localScale = original.lossyScale;
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
