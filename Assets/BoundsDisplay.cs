using UnityEngine;

[ExecuteAlways]
public class BoundsDisplay : MonoBehaviour
{
    Renderer rend;
    Mesh mesh;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mesh = GetComponent<MeshFilter>().mesh;
    }



    // Draws a wireframe sphere in the Scene view, fully enclosing
    // the object.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, rend.bounds.size);
    }
}
