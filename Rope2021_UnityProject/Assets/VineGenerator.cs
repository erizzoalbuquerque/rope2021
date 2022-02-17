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

    [SerializeField] GameObject _vineBonePrefab;
    [SerializeField] GameObject _vineTipPrefab;
     
    private LineRenderer _lineRenderer;

    public  List<GameObject> _bones;
    public  GameObject _vineTip;

    private float _lastVineSize = 1f;

    //private bool _vineIsGenerated = false;


    void Start()
    {
        print("Start");
        _lineRenderer = GetComponent<LineRenderer>();
        _lastVineSize = _vineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            if (_lastVineSize != _vineSize)
            {
                _lastVineSize = _vineSize;
                GenerateVine();
            }
        }

        if (Application.isPlaying)
        {
            UpdateVinePoints();
        }
    }

    [ContextMenu("GenerateVine")]
    public void GenerateVine()
    {
        print("Gen");
        if (_vineBonePrefab == null || _vineTipPrefab == null)
            return;

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
        CreateVineTip();

        //_vineIsGenerated = true;
    }

    void CreateBones()
    {
        print("Create");
        DeleteBones();

        _bones = new List<GameObject>();

        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            GameObject newBone = Instantiate(_vineBonePrefab, this.transform);
            newBone.name = "VineBone_" + i.ToString();

            //Set Position
            newBone.transform.position = this.transform.TransformPoint(_lineRenderer.GetPosition(i));

            _bones.Add(newBone);
        }

        //Set-up Joints
        for (int i = 0; i < _bones.Count; i++)
        {
            if (i != 0)
            {
                HingeJoint2D joint = _bones[i].GetComponent<HingeJoint2D>();
                joint.connectedBody = _bones[i - 1].GetComponent<Rigidbody2D>();
            }
        }
    }

    void DeleteBones()
    {
        if (_bones == null)
            return;

        for (int i = 0; i < _bones.Count; i++)
        {
            DestroyImmediate(_bones[i]);
        }

        _bones.Clear();
    }

    void UpdateVinePoints()
    {
        if (_bones == null || _bones.Count == 0 || _vineTip == null)
            return;

        print("UpV");
        Vector3[] positions = new Vector3[_bones.Count];
        for (int i = 0; i < _bones.Count; i++)
        {
            positions[i] = this.transform.InverseTransformPoint(_bones[i].transform.position);
        }

        _lineRenderer.SetPositions(positions);
    }


    void CreateVineTip()
    {
        DeleteVineTip();

        _vineTip = Instantiate(_vineTipPrefab,this.transform);
        _vineTip.name = "VineTip";

        //Set Position
        _vineTip.transform.position = this.transform.TransformPoint(_lineRenderer.GetPosition(_lineRenderer.positionCount-1));

        //Set-up Joints
        FixedJoint2D joint = _vineTip.GetComponent<FixedJoint2D>();
        joint.connectedBody = _bones[_lineRenderer.positionCount - 1].GetComponent<Rigidbody2D>();
    }

    void DeleteVineTip()
    {
        if (_vineTip == null)
            return;

        DestroyImmediate(_vineTip);
    }
}
