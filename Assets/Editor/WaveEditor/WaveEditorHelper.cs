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

    internal static GameObject BuildUnmovableMeshCopy(GameObject original, string name, string tag = "Untagged")
    {
        var go = new GameObject("SpawnCopy");
        go.tag = tag;

        var meshf = go.AddComponent<MeshFilter>();
        meshf.mesh = original.GetComponent<MeshFilter>().sharedMesh;

        var rend = go.AddComponent<MeshRenderer>();
        rend.material = original.GetComponent<MeshRenderer>().sharedMaterial;

        go.AddComponent<ImmutableGameObject>();
        return go;
    }
}
