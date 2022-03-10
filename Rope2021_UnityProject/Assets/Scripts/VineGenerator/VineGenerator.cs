using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatizes the creation of a Vine of variable size. Should be used with the pre configured prefab.
/// 
/// TODO: Separte this class in Generator and Updater
/// </summary>
[ExecuteInEditMode]
public class VineGenerator : MonoBehaviour
{
    [SerializeField] float _vineSize = 1f;
    [SerializeField] float _aproxSizeBetweenPoints = 1f;

    [SerializeField] GameObject _vineBonePrefab;
    [SerializeField] GameObject _vineTipPrefab;

    private LineRenderer _lineRenderer;
    [ReadOnly,SerializeField]   private List<GameObject> _bones;
    [ReadOnly, SerializeField] private GameObject _vineTip;

    private float _lastVineSize = 1f;

    //private bool _vineIsGenerated = false;


    void Start()
    {
        ////Any modification on this Prefab shouldn't be saved, so we unpack it.
        //if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this.gameObject)) 
        //    UnityEditor.PrefabUtility.UnpackPrefabInstance(this.gameObject, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);       

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

        //print("Created new bones to vine:" + this.gameObject.name);
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
        if (_bones == null || _bones.Count == 0)
            return;

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
        _vineTip = null;
    }

    /// <summary>
    /// Delete any child of this game object and generate the vine again. Also, update references.
    /// </summary>
    [ContextMenu("Repair")]
    void Reapair()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        DeleteBones();
        DeleteVineTip();

        List<Transform> children = new List<Transform>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children.Add(this.transform.GetChild(i));
        }

        foreach (Transform c in children)
        {
            DestroyImmediate(c.gameObject);
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(1, Vector2.down);

        GenerateVine();
    }
}
