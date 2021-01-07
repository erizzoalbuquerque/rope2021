using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEngine.ProBuilder;

public static class SetPolygonCollider3D
{
    [MenuItem("Tools/Update Polygon Colliders %t", false, -1)]
    static void UpdatePolygonColliders()
    {
        Transform transform = Selection.activeTransform;
        if (transform == null)
        {
            Debug.LogWarning("No valid GameObject selected!");
            return;
        }

        EditorSceneManager.MarkSceneDirty(transform.gameObject.scene);

        MeshCollider[] meshColliders = transform.GetComponentsInChildren<MeshCollider>();

        foreach (MeshCollider meshCollider in meshColliders)
        {
            UpdatePolygonCollider2D(meshCollider);
        }
    }

    static void UpdatePolygonCollider2D(MeshCollider meshCollider)
    {
        if (meshCollider.sharedMesh == null)
        {
            Debug.LogWarning(meshCollider.gameObject.name + " has no Mesh set on its MeshFilter component!");
            return;
        }

        //UnityEngine.ProBuilder.ProBuilderMesh pbmesh = new UnityEngine.ProBuilder.ProBuilderMesh();
        //UnityEngine.ProBuilder.Vertex[] vertex = pbmesh.GetVertices();

        List<Vector3> vertices = new List<Vector3>();
        meshCollider.sharedMesh.GetVertices(vertices);

        Debug.Log(vertices.ToString());

        var boundaryPath = EdgeHelpers.GetEdges(meshCollider.sharedMesh.triangles).FindBoundary().SortEdges();

        Vector3[] yourVectors = new Vector3[boundaryPath.Count];
        for (int i = 0; i < boundaryPath.Count; i++)
        {
            yourVectors[i] = vertices[boundaryPath[i].v1];
        }
        List<Vector2> newColliderVertices = new List<Vector2>();

        for (int i = 0; i < yourVectors.Length; i++)
        {
            newColliderVertices.Add(new Vector2(yourVectors[i].x, yourVectors[i].y));
        }

        Vector2[] newPoints = newColliderVertices.Distinct().ToArray();

        for (int i=0; i < meshCollider.transform.childCount; i++)
        {
            GameObject go = meshCollider.transform.GetChild(i).gameObject;
            if (go.name == "Collider2dGO")
                UnityEngine.Object.DestroyImmediate(go);
        }
        
        GameObject collider2dGO = new GameObject("Collider2dGO");
        collider2dGO.transform.SetParent(meshCollider.gameObject.transform);
        
        collider2dGO.transform.localPosition = Vector3.zero;
        collider2dGO.transform.localRotation = Quaternion.identity;
        collider2dGO.transform.localScale = Vector3.one;
        collider2dGO.layer = LayerMask.NameToLayer("Solid");

        PolygonCollider2D polygonCollider2D = collider2dGO.AddComponent<PolygonCollider2D>();

        //PolygonCollider2D polygonCollider2D = meshCollider.GetComponent<PolygonCollider2D>();
        polygonCollider2D.pathCount = 1;

        EditorUtility.SetDirty(polygonCollider2D);

        polygonCollider2D.SetPath(0, newPoints);
        Debug.Log(meshCollider.gameObject.name + " PolygonCollider2D updated.");
    }
}

public static class EdgeHelpers
{
    public struct Edge
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