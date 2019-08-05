using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareWaypoint : MonoBehaviour
{
    public Bounds Area { get; set; }

    [SerializeField]
    public SquareWaypoint Next;

    void Start()
    {
        FindAndSetInnerBounds();
    }

    private void OnDrawGizmosSelected()
    {
        FindAndSetInnerBounds();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Area.center, Area.size * 0.995f);
    }

    public void FindAndSetInnerBounds()
    {
        Mesh objmesh = GetComponent<MeshFilter>().sharedMesh;

        List<Vector3> v = new List<Vector3>();

        foreach (var item in objmesh.vertices.Distinct().ToList())
        {
            v.Add(transform.TransformPoint(item));
        }

        Area = new Bounds(transform.position, Vector3.zero);

        foreach (var p in FindCornersClosestsToMiddle(v))
        {
            Area.Encapsulate(p);
        }

        //DrawBallAtPoints(FindCornersClosestsToMiddle(v));
    }

    private IEnumerable<Vector3> FindCornersClosestsToMiddle(IEnumerable<Vector3> points)
    {
        Dictionary<Vector3, float> innerpoints = new Dictionary<Vector3, float>();
        const int pointsmax = 4;

        foreach (var point in points)
        {
            innerpoints.Add(point, Vector3.Distance(point, transform.position));
        }

        Dictionary<Vector3, float> ordered = innerpoints.OrderBy(x => x.Value).ToDictionary(t => t.Key, t => t.Value);

        List<Vector3> vectors = new List<Vector3>();

        int index = 0;
        while (vectors.Count < pointsmax)
        {
            vectors.Add(ordered.Keys.ToArray()[index]);
            index++;
        }

        return vectors;
    }

    //private void DrawBallAtPoints(IEnumerable<Vector3> points)
    //{
    //    int index = 0;
    //    foreach (var vert in points)
    //    {
    //        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    //        ball.transform.localScale = Vector3.one * 0.2f;

    //        ball.transform.position = vert;
    //        ball.name = $"Vert{index}";
    //        index++;
    //    }
    //}
}
