using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Create a PolygonCollider2D using a MeshCollider3D as reference. The PolygonCollider is created as a child of the Game Object. Also, if the
/// Mesh Collider is enabled, it is disabled. This code doesn't work for meshes with a hole whitin.
/// Based in this algorithm found online:
/// https://www.h3xed.com/programming/automatically-create-polygon-collider-2d-from-2d-mesh-in-unity
/// </summary>
public class ColliderCreator
{
    static readonly string GAMEOBJECTHOLDERNAME = "PolygonColliderHolder";

    [MenuItem("Tools/CreatePolygonColliderFromMeshCollider %t", false, -1)]
    static void UpdatePolygonColliders()
    {
        Debug.Log("Initializing CreatePolygonColliderFromMeshCollider.");

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
            Debug.Log("Started Creating PolygonCollider2D for " + meshCollider.name + "...");

            Mesh mesh = meshCollider.sharedMesh;

            CleanAnyHolderInRoot(meshCollider.transform);
            Transform polygonColliderHolder = GetHolderTransform(meshCollider.transform);

            CreatePolygonColliderFromMesh(mesh, polygonColliderHolder);

            Debug.Log(meshCollider.name + " PolygonCollider2D created SUCCESFULLY.");
        }

        Debug.Log("CreatePolygonColliderFromMeshCollider done.");
    }

    static void CleanAnyHolderInRoot(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.childCount; i++)
        {
            GameObject go = rootTransform.GetChild(i).gameObject;
            if (go.name == GAMEOBJECTHOLDERNAME)
                UnityEngine.Object.DestroyImmediate(go);
        }
    }

    static Transform GetHolderTransform(Transform rootTransform)
    {
        GameObject holderGO = new GameObject(GAMEOBJECTHOLDERNAME);

        holderGO.transform.SetParent(rootTransform);

        holderGO.transform.localPosition = Vector3.zero;
        holderGO.transform.localRotation = Quaternion.identity;
        holderGO.transform.localScale = Vector3.one;

        holderGO.layer = LayerMask.NameToLayer("Solid");

        return holderGO.transform;
    }

    static void CreatePolygonColliderFromMesh(Mesh mesh, Transform holder)
    {
        // Get triangles and vertices from mesh
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edge))
                {
                    edges.Remove(edge);
                }
                else
                {
                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }

        // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
        Dictionary<int, int> lookup = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
            }
        }

        // Create empty polygon collider
        PolygonCollider2D polygonCollider = holder.gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.pathCount = 0;

        // Loop through edge vertices in order
        int startVert = 0;
        int nextVert = startVert;
        int highestVert = startVert;
        List<Vector2> colliderPath = new List<Vector2>();
        // NEW CODE BEGIN ---------------------------
        int longerPathCount = 0;
        // NEW CODE END ---------------------------

        while (true)
        {

            // Add vertex to collider path
            colliderPath.Add(vertices[nextVert]);

            // Get next vertex
            nextVert = lookup[nextVert];

            // Store highest vertex (to know what shape to move to next)
            if (nextVert > highestVert)
            {
                highestVert = nextVert;
            }

            // Shape complete
            if (nextVert == startVert)
            {
                // OLD CODE BEGIN ---------------------------
                //polygonCollider.pathCount++;
                //polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                // OLD CODE END ---------------------------

                // NEW CODE BEGIN ---------------------------
                if (colliderPath.Count > longerPathCount)
                {
                    longerPathCount = colliderPath.Count;
                    polygonCollider.SetPath(0, colliderPath.ToArray());
                }
                // NEW CODE END ---------------------------

                colliderPath.Clear();

                // Go to next shape if one exists
                if (lookup.ContainsKey(highestVert + 1))
                {

                    // Set starting and next vertices
                    startVert = highestVert + 1;
                    nextVert = startVert;

                    // Continue to next loop
                    continue;
                }

                // No more verts
                break;
            }
        }
    }
}