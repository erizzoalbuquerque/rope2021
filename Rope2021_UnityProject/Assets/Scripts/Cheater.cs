using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheater : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;

    Vector3 cameraStartLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        cameraStartLocalPosition = cameraTarget.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            cameraTarget.localPosition = new Vector3(cameraStartLocalPosition.x, cameraStartLocalPosition.y, cameraStartLocalPosition.z);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            cameraTarget.localPosition = new Vector3(cameraStartLocalPosition.x, cameraStartLocalPosition.y, cameraStartLocalPosition.z * 3f);

        if (Input.GetKeyDown(KeyCode.F4))
            ResetScene();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
