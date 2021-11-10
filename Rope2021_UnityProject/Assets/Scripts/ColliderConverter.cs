using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColliderConverter : MonoBehaviour
{
    readonly string GAMEOBJECTHOLDERNAME = "PolygonColliderHolder";

    Vector3[] vertices;
    List<EdgeHelpers.Edge> boundaryEdges;
    List<EdgeHelpers.Edge> externalBoundaryEdges;
    List<int> verticesIndexPath;

    [SerializeField] int numberOfEdgesToShow = 10000;
    [SerializeField] int numberOfEdgesOnThePathToShow = 10000;


    [ContextMenu("UpdateCollider")]
    void UpdateCollider()
    {
        CleanAnyHolderInRoot(this.transform);
        
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        GeneratePolygonPathFromMesh(mesh);

        Transform holder = GetHolderTransform(this.transform);

        CreatePolygonCollider(holder);
    }

    void GeneratePolygonPathFromMesh(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        vertices = mesh.vertices;

        List<EdgeHelpers.Edge> edges = EdgeHelpers.GetEdges(triangles);

        boundaryEdges = new List<EdgeHelpers.Edge>();
        boundaryEdges = edges.FindBoundary();

        externalBoundaryEdges = new List<EdgeHelpers.Edge>();

        foreach (EdgeHelpers.Edge edge in boundaryEdges)
        {
            if (transform.TransformPoint(vertices[edge.v1]).z > 0f && transform.TransformPoint(vertices[edge.v2]).z > 0f)
                externalBoundaryEdges.Add(edge);
        }

        externalBoundaryEdges = externalBoundaryEdges.SortEdges();
        print(externalBoundaryEdges.Count);

        verticesIndexPath = new List<int>();
        List<EdgeHelpers.Edge> remainingEdges = new List<EdgeHelpers.Edge>(externalBoundaryEdges);

        int abortCounter = 0;
        bool edgeFound = true;
        int currentHead = remainingEdges[0].v1;
        remainingEdges.Remove(remainingEdges[0]);
        verticesIndexPath.Add(currentHead);
        while (remainingEdges.Count > 0 && edgeFound && abortCounter < externalBoundaryEdges.Count)
        {
            EdgeHelpers.Edge nextEdge = null;
            foreach (EdgeHelpers.Edge edge in remainingEdges)
            {
                if (CompareVector3(vertices[edge.v1], vertices[currentHead]))
                {
                    nextEdge = edge;
                    break;
                }
                else if (CompareVector3(vertices[edge.v2], vertices[currentHead]))
                {
                    nextEdge = edge;
                    break;
                }
            }


            if (nextEdge != null)
            {
                remainingEdges.Remove(nextEdge);

                currentHead = CompareVector3(vertices[nextEdge.v1], vertices[currentHead]) ? nextEdge.v2 : nextEdge.v1;

                verticesIndexPath.Add(currentHead);
            }
            else
            {
                edgeFound = false;
            }

            abortCounter++;
        }
        print(verticesIndexPath.Count);
    }

    void CreatePolygonCollider(Transform holder)
    {
        PolygonCollider2D polygonCollider2D = holder.gameObject.AddComponent<PolygonCollider2D>();

        polygonCollider2D.pathCount = 1;

        List<Vector2> path = new List<Vector2>();

        for (int i = 0; i < verticesIndexPath.Count; i++)
        {
            Vector2 point = new Vector2(vertices[verticesIndexPath[i]].x, vertices[verticesIndexPath[i]].y);
            if (!path.Contains(point))
                path.Add(new Vector2(vertices[verticesIndexPath[i]].x, vertices[verticesIndexPath[i]].y));
        }


        polygonCollider2D.SetPath(0, path);
    }


    bool CompareVector3(Vector3 a, Vector3 b,float maxDist = 0.05f)
    {
        return Vector3.Distance(a, b) <= maxDist ? true : false;
    }

    void CleanAnyHolderInRoot(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.childCount; i++)
        {
            GameObject go = rootTransform.GetChild(i).gameObject;
            if (go.name == GAMEOBJECTHOLDERNAME)
                UnityEngine.Object.DestroyImmediate(go);
        }
    }

    Transform GetHolderTransform(Transform rootTransform)
    {
        GameObject holderGO = new GameObject(GAMEOBJECTHOLDERNAME);

        holderGO.transform.SetParent(rootTransform);

        holderGO.transform.localPosition = Vector3.zero;
        holderGO.transform.localRotation = Quaternion.identity;
        holderGO.transform.localScale = Vector3.one;

        holderGO.layer = LayerMask.NameToLayer("Solid");

        return holderGO.transform;
    }


    private void OnDrawGizmos()
    {
        if (externalBoundaryEdges == null || externalBoundaryEdges.Count == 0)
            return;

        int count = System.Math.Min(externalBoundaryEdges.Count, numberOfEdgesToShow);
        for (int i = 0; i < count; i++ )
        {
            EdgeHelpers.Edge edge = externalBoundaryEdges[i];
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.TransformPoint(vertices[edge.v1]), transform.TransformPoint(vertices[edge.v2]));
        }

        count = System.Math.Min(verticesIndexPath.Count, numberOfEdgesOnThePathToShow);
        for (int i = 0; i < count-1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.TransformPoint(vertices[verticesIndexPath[i]]), transform.TransformPoint(vertices[verticesIndexPath[i+1]]));
        }

    }
}

public static class EdgeHelpers
{
    public class Edge
    {
        public int v1;
        public int v2;
        public int triangleIndex;
        public Edge(int aV1, int aV2, int aIndex)
        {
            v1 = aV1;
            v2 = aV2;
            triangleIndex = aIndex;
        }
    }

    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            result.Add(new Edge(v1, v2, i));
            result.Add(new Edge(v2, v3, i));
            result.Add(new Edge(v3, v1, i));
        }
        return result;
    }

    public static List<Edge> FindBoundary(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }
    public static List<Edge> SortEdges(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = 0; i < result.Count - 2; i++)
        {
            Edge E = result[i];
            for (int n = i + 1; n < result.Count; n++)
            {
                Edge a = result[n];
                if (E.v2 == a.v1)
                {
                    // in this case they are already in order so just continoue with the next one
                    if (n == i + 1)
                        break;
                    // if we found a match, swap them with the next one after "i"
                    result[n] = result[i + 1];
                    result[i + 1] = a;
                    break;
                }
            }
        }
        return result;
    }
}
