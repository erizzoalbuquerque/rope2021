using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Class used to quickly move a game object across scene. CTRL + MOUSE MIDDLE BUTTON to activate.
/// </summary>
[ExecuteInEditMode]
public class SpawPointEditorTool : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    public Vector3 spawnPosition = Vector3.zero;

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
            return;
        }
        SceneView.duringSceneGui += OnScene;
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 2 && e.control)
        {
            Vector3 mousePos = e.mousePosition;
            Ray ray = scene.camera.ScreenPointToRay(new Vector3(mousePos.x, scene.camera.pixelHeight - mousePos.y, mousePos.z));

            Plane plane = new Plane(Vector3.back, 0);
            float enter = 0.0f;
            plane.Raycast(ray, out enter);

            ray.GetPoint(enter);

            spawnPosition = ray.GetPoint(enter);

            playerTransform.position = spawnPosition;

            //e.Use();
        }

        //if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl && e.keyCode == KeyCode.LeftAlt && e.keyCode == KeyCode.Space)
        //{
        //    playerTransform.position = spawnPosition;
        //
        //    //e.Use();
        //}
    }
}
#endif