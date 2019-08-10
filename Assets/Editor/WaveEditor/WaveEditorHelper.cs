using UnityEngine;

internal static class WaveEditorHelper
{
    internal static int CalculateRows(int columns, int buttonscount)
    {
        int buttonstodraw = buttonscount;

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
        var go = new GameObject(original.name);

        go.transform.localScale = original.transform.lossyScale;

        var meshf = go.AddComponent<MeshFilter>();
        meshf.mesh = original.GetComponent<MeshFilter>().sharedMesh;

        var rend = go.AddComponent<MeshRenderer>();
        rend.material = original.GetComponent<MeshRenderer>().sharedMaterial;

        return go;
    }
}
