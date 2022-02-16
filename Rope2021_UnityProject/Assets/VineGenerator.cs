using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatizes the creation of a Vine o variable size. Should be used with the pre configured prefab.
/// </summary>
[ExecuteInEditMode]
public class VineGenerator : MonoBehaviour
{
    [SerializeField] float _vineSize = 1f;
    [SerializeField] float _aproxSizeBetweenPoints = 1f;
    [SerializeField] Vector2 _vineTipOffset = Vector2.zero;

    [SerializeField] GameObject _vineBonePrefab;
    [SerializeField] Transform _vineTip;
    
 
    private LineRenderer _lineRenderer;

    public  List<GameObject> bones;

    private float _lastVineSize = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lastVineSize = _vineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            if (_lastVineSize != _vineSize)
                ResizeVine();
        }

        UpdateVinePoints();
    }

    [ContextMenu("GenerateVine")]
    public void GenerateVine()
    {
        int numberOfPoints = Mathf.RoundToInt(_vineSize / _aproxSizeBetweenPoints);

        if (numberOfPoints < 2)
        {
            numberOfPoints = 2;
        }

        Vector3[] positions = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            positions[i] = Vector3.down * _vineSize/numberOfPoints * i;
            //print("point " + i.ToString() + " at " + positions[i].ToString());
        }

        _lineRenderer.positionCount = numberOfPoints;
        _lineRenderer.SetPositions(positions);

        CreateBones();
        PlaceVineTip();
    }

    void CreateBones()
    {
        DeleteBones();

        bones = new List<GameObject>();

        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            GameObject newBone = Instantiate(_vineBonePrefab, this.transform);
            newBone.name = "VineBone_" + i.ToString();

            //Set Position
            newBone.transform.position = this.transform.TransformPoint(_lineRenderer.GetPosition(i));

            bones.Add(newBone);
        }

        //Set-up Joints
        for (int i = 0; i < bones.Count; i++)
        {
            if (i != 0)
            {
                HingeJoint2D joint = bones[i].GetComponent<HingeJoint2D>();
                joint.connectedBody = bones[i - 1].GetComponent<Rigidbody2D>();
            }
        }
    }

    void DeleteBones()
    {
        for (int i = 0; i < bones.Count; i++)
        {
            DestroyImmediate(bones[i]);
        }

        bones.Clear();
    }

    void UpdateVinePoints()
    {
        Vector3[] positions = new Vector3[bones.Count];
        for (int i = 0; i < bones.Count; i++)
        {
            positions[i] = this.transform.InverseTransformPoint(bones[i].transform.position);
        }

        _lineRenderer.SetPositions(positions);
    }

    void ResizeVine()
    {
        //_lineRenderer.SetPosition(_numberOfPoints - 1, _vineSize * Vector3.down);
        //
        //PlaceVineTip();
        //
        //_lastVineSize = _vineSize;
    }

    void PlaceVineTip()
    {
        //Set Position
        _vineTip.position = this.transform.TransformPoint(_lineRenderer.GetPosition(_lineRenderer.positionCount-1) + (Vector3)_vineTipOffset);

        //Set-up Joints
        HingeJoint2D joint = _vineTip.GetComponent<HingeJoint2D>();
        joint.connectedBody = bones[_lineRenderer.positionCount - 1].GetComponent<Rigidbody2D>();
    }
}
